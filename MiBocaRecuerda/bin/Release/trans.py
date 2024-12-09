from googletrans import Translator
import io,sys
import csv

def read_csv_lang_code(csv_file):
    lang_code = []
    with open(csv_file, newline='', encoding="utf-8") as file:
        reader = csv.reader(file)
        for row in reader:
            if row:  # 空行をスキップ
                lang_code.append(row[0])
    return lang_code

if __name__ == '__main__':

    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

    if len(sys.argv) != 4:
        print("2¶xx¶起動引数")
        sys.exit()

    lang = read_csv_lang_code("lang.csv")

    translator = Translator()
    result = translator.translate(sys.argv[1], src=lang[int(sys.argv[2])], dest=lang[int(sys.argv[3])])
    #result = translator.translate("hello", "ko")
    
    print(f'0¶{result.src}¶{result.text}¶{result.pronunciation}')
