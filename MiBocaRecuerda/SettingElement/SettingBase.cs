using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System;
using System.Drawing;

namespace MiBocaRecuerda
{
    public partial class SettingBase : UserControl
    {
        [Browsable(true)]
        [Category("表示")]
        [Description("言語設定")]
        public string LanguageName { get; set; }

        public event EventHandler SomethingChanged;

        public string SelectedFileName => cmbboxFileName.SelectedItem?.ToString();

        private ExerciseRepository _exerRepo;

        // 設定画面起動時のクイズ情報のコピー
        private Dictionary<string, QuizFileConfig> _qfc;
        private QuizFileConfig _currentQuizFileConfig => _qfc[cmbboxFileName.Text];

        private bool _isUpdating = false;

        // 設定の妥当性
        public bool IsValid { get; private set; } = false;

        public int QuizMax
        {
            get
            {
                if (_exerRepo != null)
                {
                    return _exerRepo.GetExerciseCount();
                }
                else
                {
                    return -1;
                }
            }
        }

        protected ComboBox _cmbboxFileName => cmbboxFileName;

        public SettingBase()
        {
            InitializeComponent();

            cmbboxFileName.SelectedIndexChanged += (o, e) =>
            {
                try
                {
                    _isUpdating = true;
                    _exerRepo = new ExerciseRepository($"Data Source={PathManager.QuizDB(cmbboxFileName.Text)}");
                    // MaximumをあげておいてSetValueでつぶされてしまうのを防ぐ
                    nudQuizNum.Maximum = 1000;
                    nudMinChapter.Maximum = 1000;
                    nudMaxChapter.Maximum = 1000;
                    SetValue();
                    lblQuizMax.Text = $"max: {QuizMax}";
                    nudQuizNum.Maximum = QuizMax;
                    nudMinChapter.Maximum = UtilityFunction.Techo(QuizMax, 10);
                    nudMaxChapter.Maximum = UtilityFunction.Techo(QuizMax, 10);
                    ChangeEnabled(true);
                }
                finally
                {
                    _isUpdating = false;
                }
            };

            nudMinChapter.ValueChanged += NudMinValueChanged;
            nudMinChapter.ValueChanged += QuizNudValueChanged_common;

            nudMaxChapter.ValueChanged += NudMaxValueChanged;
            nudMaxChapter.ValueChanged += QuizNudValueChanged_common;

            nudQuizNum.ValueChanged += NudQuizNumChanged;
            nudQuizNum.ValueChanged += QuizNudValueChanged_common;

            UpdateErrorControls();
        }

        public virtual void ChangeEnabled(bool isEnabled)
        {
            tabControl1.Enabled = isEnabled;
        }

        // 最小チャプター変更
        private void NudMinValueChanged(object o, EventArgs e)
        {
            if (_isUpdating) return;

            if (nudMinChapter.Value > nudMaxChapter.Value)
            {
                nudMaxChapter.Value = nudMinChapter.Value;
            }
            _currentQuizFileConfig.MinChapter = (int)nudMinChapter.Value;
        }

        // 最大チャプター変更
        private void NudMaxValueChanged(object o, EventArgs e)
        {
            if (_isUpdating) return;

            if (nudMaxChapter.Value < nudMinChapter.Value)
            {
                nudMinChapter.Value = nudMaxChapter.Value;
            }
            _currentQuizFileConfig.MaxChapter = (int)nudMaxChapter.Value;
        }

        private void NudQuizNumChanged(object o, EventArgs e)
        {
            if (_isUpdating) return;

            _currentQuizFileConfig.QuizNum = (int)nudQuizNum.Value;
        }

        // 問題数関係変更
        private void QuizNudValueChanged_common(object o, EventArgs e)
        {
            int quizNum = (int)nudQuizNum.Value;
            int overflow = 0;

            // 問題許容数を超過する場合に溢れ分をとる
            if (quizNum - _currentQuizFileConfig.PermitNum > 0)
            {
                overflow = quizNum - _currentQuizFileConfig.PermitNum;
            }

            lblQuizRange.Text = $"range: {(nudMinChapter.Value - 1) * 10 + 1}~{(_currentQuizFileConfig.IsMaxChapter ? QuizMax : nudMaxChapter.Value * 10) + overflow}";

            // あふれがないときに設定が妥当とする
            IsValid = !(overflow > 0);

            lblQuizRange.ForeColor = IsValid ? Color.Black : Color.Red;

            // 何かの変更があったことをSetting Formに通知
            SomethingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetValue()
        {
            nudMinChapter.Value = _currentQuizFileConfig.MinChapter;
            nudMaxChapter.Value = _currentQuizFileConfig.MaxChapter;
            nudQuizNum.Value = _currentQuizFileConfig.QuizNum;
            nudErrorAllow.Value = _currentQuizFileConfig.ErrorAllowCnt;
            chboxErrorAllowAll.Checked = _currentQuizFileConfig.ErrorAllowAll;
            chboxErrorReset.Checked = _currentQuizFileConfig.ErrorReset;
            cmbRegion.SelectedItem = _currentQuizFileConfig.PriorityRegion;
        }

        public virtual void LoadConfig(string currentFile)
        {
            _qfc = SettingManager.GetAllQuizFileConfig(LanguageName);

            cmbboxFileName.Items.AddRange(_qfc.Keys.ToArray());

            cmbboxFileName.SelectedItem = currentFile;
        }

        public QuizFileConfig GetCommon()
        {
            QuizFileConfig lang = new QuizFileConfig
            {
                MinChapter = (int)nudMinChapter.Value,
                MaxChapter = (int)nudMaxChapter.Value,
                QuizNum = (int)nudQuizNum.Value,
                ErrorAllowCnt = (int)nudErrorAllow.Value,
                ErrorAllowAll = chboxErrorAllowAll.Checked,
                ErrorReset = chboxErrorReset.Checked,
                PriorityRegion = cmbRegion.SelectedItem.ToString(),
                MaxQuizNum = _exerRepo.GetExerciseCount()
            };

            return lang;
        }

        protected void SetRegion(string[] region)
        {
            cmbRegion.Items.AddRange(region);
        }

        // LenguaConfigは継承先で設定するため、このメソッドは継承先で必ず実装すること
        // SettingFormで共通で使うために、ここで宣言しておく必要があった
        public virtual FileLenguaConfig GetLang() { return new FileLenguaConfig(); }

        private void btnQuizMax_Click(object sender, System.EventArgs e)
        {
            nudQuizNum.Value = _currentQuizFileConfig.PermitNum;
        }

        private void chboxErrorAllowAll_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateErrorControls();
        }

        private void chboxErrorReset_CheckedChanged(object sender, System.EventArgs e)
        {
            UpdateErrorControls();
        }

        private void UpdateErrorControls()
        {
            bool check_eaa = chboxErrorAllowAll.Checked;
            bool check_er = chboxErrorReset.Checked;
            int error_cnt = (int)nudErrorAllow.Value;

            if (error_cnt > 0)
            {
                lblErrorAll.Text = $"ミス許容範囲：{(check_eaa ? "セッション" : "問題")}";
                lblErrorReset.Text = $"ミス満了リセット：{(check_er ? "はい" : "いいえ")}";

                chboxErrorAllowAll.Enabled = true;
                chboxErrorReset.Enabled = true;
            }
            else
            {
                lblErrorAll.Text = $"ミス許容範囲：-";
                lblErrorReset.Text = $"ミス満了リセット：-";

                chboxErrorAllowAll.Enabled = false;
                chboxErrorReset.Enabled = false;
            }
        }

        private void nudErrorAllow_ValueChanged(object sender, System.EventArgs e)
        {
            UpdateErrorControls();
        }
    }
}
