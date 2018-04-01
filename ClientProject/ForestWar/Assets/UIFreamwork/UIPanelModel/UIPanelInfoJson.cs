using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.UIFreamwork.UIPanel
{
    /// <summary>
    /// 该类用于接收JsonUtility解析的数据集合
    /// </summary>
    [Serializable]
    class UIPanelInfoJson
    {
        public List<UIPanelInfo> panelInfoList;
    }
}
