using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


/// <summary>
/// csvの内容をstring(key)int(body)の辞書したリストに変換するクラス
/// </summary>
public class CsvReader
{
    private List<Dictionary<string, int>> _csvData;
    public List<Dictionary<string, int>> CsvData => _csvData;

    /// <summary>
    /// CSVを読んでデータのかたまりを作る
    /// </summary>
    /// 引数をstringじゃなくて、enumとかにしたい　どっかに辞書定義して
    public CsvReader Create(string csvName)
    {
        // 本当はこういうマジックナンバーを存在させていはいけない
        var csvFile = Resources.Load<TextAsset>("csv/" + csvName);
        if (csvFile is null)
        {
            Debug.LogError("csvファイルが存在しません:" + csvName);
            return null;
        }

        // いったんCSVの内容を全部よみだす
        StringReader reader = new StringReader(csvFile.text);
        var tempCsvData = new List<string[]>();
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            tempCsvData.Add(line.Split(",")); // カンマ区切りで読み込み
        }

        // Dictionary格納用に成形
        // 1行目はカラム名なので2行目から読む
        _csvData = new List<Dictionary<string, int>>();
        for (var i = 1; i < tempCsvData.Count; i++)
        {
            // 一列ぶんのデータの辞書作成
            var dict = tempCsvData[i].Select((data, index) =>
            {
                var key = tempCsvData[0][index];
                var value = data;
                return (key, value);
            })
            .ToDictionary(data => data.key, data => int.Parse(data.value));

            _csvData.Add(dict);
        }

        return this;
    }
}
