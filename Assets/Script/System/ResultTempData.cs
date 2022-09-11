using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultTempData 
{
    public class Parameter
    {
        public bool IsClear;
        public int FinalCharaLevel;
        public List<int> ItemIdList;
        public int Score;
        public int SpecialAttackNum;
    }

    public static ResultTempData Instance => _instance;
    private static ResultTempData _instance = new ResultTempData();

    private Parameter _parameter;

    public void SetData(Parameter param)
    {
        _parameter = param;
    }

    public Parameter GetData()
    {
        return _parameter;
    }
}
