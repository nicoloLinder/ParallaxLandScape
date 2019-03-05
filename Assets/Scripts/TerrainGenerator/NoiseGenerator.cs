using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Initialize segment
//While iterations<num_of_iterations and segments_length > min_length:

//For each segment:

//Compute midpoint

//Displace midpoint

//Update segments
//Reduce displacement bounds
//iterations+1

public static class NoiseGenerator
{

    #region Variables

    #region PublicVariables

    #endregion

    #region PrivateVariables

    #endregion

    #endregion

    #region Properties

    #endregion

    #region Methods

    #region PublicMethods

    public static Vector2[] GenerateNoiseMap(int numberOfIterations, Vector2 start, Vector2 end, float roughness, float verticalDisplacement)
    {
        Vector2[] noiseMap = { start, end };

        if (Mathf.Abs(verticalDisplacement) < float.MinValue)
        {
            verticalDisplacement = (start[1] + end[1]) / 2;
        }

        Vector2 midPoint;

        for (int i = 0; i < numberOfIterations; i++)
        {
            Vector2[] tempNoiseMap = new Vector2[noiseMap.Length + (noiseMap.Length - 1)];

            int k = 0;
            for (int j = 0; j < noiseMap.Length-1; j++)
            {
                tempNoiseMap[k++] = noiseMap[j];

                midPoint = (noiseMap[j] + noiseMap[j + 1]) / 2;
                midPoint += Vector2.up * Random.Range(-verticalDisplacement, verticalDisplacement);

                tempNoiseMap[k++] = midPoint;
            }
            tempNoiseMap[k] = noiseMap[noiseMap.Length - 1];

            noiseMap = tempNoiseMap;
            verticalDisplacement *= Mathf.Pow(2, -roughness);
        }

        return noiseMap;
    }

    #endregion

    #region PrivateMethods

    #endregion

    #endregion

    #region Coroutines

    #endregion

}
