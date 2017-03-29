using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minefield : MonoBehaviour
{
    public GameObject MinePrefab;

    private RectTransform mTransform;

    public void GenerateField(int width, int height)
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        for(int y = 0; y < height; y++)
            for(int x = 0; x < width; x++)
            {

            }
    }

    void Start()
    {
        mTransform = GetComponent<RectTransform>();
    }
}