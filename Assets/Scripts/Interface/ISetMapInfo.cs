using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISetMapInfo 
{

    /// <summary>
    /// SectionDTOリストからNodeDTOリストへ変換
    /// </summary>
    /// <returns></returns>
    public void SetNodeDTOList(SectionDTO[,] stageObjs);

}
