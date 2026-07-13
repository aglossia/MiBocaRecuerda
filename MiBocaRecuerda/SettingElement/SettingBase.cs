using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System;
using System.Drawing;

namespace MiBocaRecuerda
{
    public partial class SettingBase : UserControl
    {
        [Browsable(true)]
        [Category("表示")]
        [Description("言語設定")]

        public event EventHandler SomethingChanged;

        public string LanguageName { get; set; }

        public string SelectedFileName => cmbboxFileName.SelectedItem?.ToString();

        private ExerciseRepository _exerRepo;

        // 妥当性検証用
        private QuizFileConfig _currentQuizFileConfig = new QuizFileConfig();
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

        private List<QuizFileConfig> _qfc;

        protected ComboBox _cmbboxFileName => cmbboxFileName;

        public SettingBase()
        {
            InitializeComponent();

            cmbboxFileName.SelectedIndexChanged += (o, e) =>
            {
                _exerRepo = new ExerciseRepository($"Data Source={PathManager.QuizDB(cmbboxFileName.Text)}");
                nudQuizNum.Maximum = QuizMax;
                nudMinChapter.Maximum = UtilityFunction.Techo(QuizMax, 10);
                nudMaxChapter.Maximum = UtilityFunction.Techo(QuizMax, 10);
                SetValue(_qfc[cmbboxFileName.SelectedIndex]);
                lblQuizMax.Text = $"max: {QuizMax}";
                ChangeEnabled(true);
            };

            nudMinChapter.ValueChanged += NudMinValueChanged;
            nudMinChapter.ValueChanged += QuizNudValueChanged_common;

            nudMaxChapter.ValueChanged += NudMaxValueChanged;
            nudMaxChapter.ValueChanged += QuizNudValueChanged_common;

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
            if (nudMinChapter.Value > nudMaxChapter.Value)
            {
                nudMaxChapter.Value = nudMinChapter.Value;
            }
        }

        // 最大チャプター変更
        private void NudMaxValueChanged(object o, EventArgs e)
        {
            if (nudMaxChapter.Value < nudMinChapter.Value)
            {
                nudMinChapter.Value = nudMaxChapter.Value;
            }
        }

        // 問題数関係変更
        private void QuizNudValueChanged_common(object o, EventArgs e)
        {
            _currentQuizFileConfig.MinChapter = (int)nudMinChapter.Value;
            _currentQuizFileConfig.MaxChapter = (int)nudMaxChapter.Value;
            _currentQuizFileConfig.MaxQuizNum = QuizMax;
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

        private void SetValue(QuizFileConfig lang)
        {
            nudMinChapter.Value = lang.MinChapter;
            nudMaxChapter.Value = lang.MaxChapter;
            nudQuizNum.Value = lang.QuizNum;
            nudErrorAllow.Value = lang.ErrorAllowCnt;
            chboxErrorAllowAll.Checked = lang.ErrorAllowAll;
            chboxErrorReset.Checked = lang.ErrorReset;
            cmbRegion.SelectedItem = lang.PriorityRegion;

            _currentQuizFileConfig.MinChapter = lang.MinChapter;
            _currentQuizFileConfig.MaxChapter = lang.MaxChapter;
            _currentQuizFileConfig.QuizNum = lang.QuizNum;
            _currentQuizFileConfig.MaxQuizNum = _exerRepo.GetExerciseCount();
        }

        public virtual void LoadConfig(string currentFile)
        {
            Dictionary<string, CommonConfig> cc = SettingManager.CommonConfigManager[LanguageName];

            _qfc = cc.Values.Select(s => s.QuizFileConfig).ToList();
            cmbboxFileName.Items.AddRange(cc.Select(p => Path.GetFileNameWithoutExtension(p.Key)).ToArray());

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
