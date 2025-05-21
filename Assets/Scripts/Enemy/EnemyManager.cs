using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{


    private SectionDTO[,] _sectionDTOs = default;
    public SectionDTO[,] SectionDTOs { set => _sectionDTOs = value; }

    // Start is called before the first frame update
    public void Start()
    {

        EnemySpawn(GetChildObj());

    }

    private async void EnemySpawn(List<GameObject> gameObjects)
    {

        for (int i = 0; i < gameObjects.Count; i++)
        {

            GameObject gameObject = gameObjects[i];
            gameObjects.Remove(gameObject);
            if(gameObject == null)
            {

                continue;

            }
            gameObject.SetActive(true);
            await Task.Delay(000);

        }

    }

    private List<GameObject> GetChildObj()
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            result.Add(child.gameObject);

        }

        return result;
    }

}
