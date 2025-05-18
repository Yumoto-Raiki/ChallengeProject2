using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IGetRoot
{

    public Task<List<Vector3>> GetRoot(Vector3 startPos, Vector3 targetPos, float climbHeight);

}
