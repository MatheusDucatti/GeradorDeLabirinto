using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    /*-----VARIÁVEIS-------------------------------------------------------------*/

    [Header("Configuration")]

    public string mapName = "Standard Map"; //Cria um GameObject (labirinto) com o nome atribuido.

    public Vector3 mapSize; //Tamanho do mapa.

    public Transform[] mazeBlocks; //Vetor com as "peças"(prefabs) que compõem o labirinto gerado.


    [Header("Modifiers")]

    public bool centralized = true; //Define como as tiles são posicionadas durante a criação do labirinto.

    /*[Range(-90, 90)]
    public int rotationTo = 0; //Quanto as tiles podem rotacionar.*/

    [Range(1, 10)]
    public int scaleTo = 1; //Quanto as tiles podem ser escalonadas.

    int[] arrayList; //Armazena uma lista de inteiros, que representam os blocos dos prefabs.

    int[] rotationArray; //Armazena os valores de rotação.

    int[,] matrixMap; //Matriz que determina o mapa do labirinto.

    int[,] rotationMap; //Determina as rotações a serem aplicadas.

    /*-----FUNÇÕES-----------------------------------------------------------------*/

    // Use this for initialization
    void Start()
    {
        MapGenerator2(); //Monta o layout mapa no inicio do script.
        MapBuilder(); //Coloca as peças nos locais determinados
    }

    //Método que desenha o mapa.
    public void MapGenerator2()
    {
        //Dividir a string a cada vírgula encontrada, converter os valores em números inteiros e armazená-los no vetor 'arrayList'.
        arrayList = Array.ConvertAll(FileManager.mapPrefab.Split(','), int.Parse);

        //Divide a string de valores rotacionais e os armazena em um array.
        rotationArray = Array.ConvertAll(FileManager.mapRotation.Split(','), int.Parse);

        //Converte os valores em float do Vector3 mapSize, e os armazena como int nas variáveis de largura e comprimento (width e length).
        int width = (int)mapSize.x;
        int length = (int)mapSize.z;

        //Inicializa a matriz com as dimensões do labirinto.
        matrixMap = new int[width, length];
        rotationMap = new int[width, length];

        //Contador local temporário;
        int cont = 0;

        //Percorre o mapa no eixo X
        for (int i = 0; i < width; i++)
        {
            //Percorre o mapa no eixo Y
            for (int j = 0; j < length; j++)
            {
                matrixMap[i, j] = arrayList[cont];
                rotationMap[i, j] = rotationArray[cont];
                cont++;
            }
        }
    }

    //Método que monta o mapa.
    public void MapBuilder()
    {
        //Garante que a construção seja bidimensional(eixos X e Z);
        mapSize = new Vector3(mapSize.x, 0f, mapSize.z);

        //Confere se há um GameObject com o mesmo nome:
        //Se achar um
        if (transform.Find(mapName))
        {
            Destroy(transform.Find(mapName).gameObject); //Destrói ele.
            Debug.LogWarning("Map object with name \"" + mapName + "\" was found and successfully terminated.");
        }

        //Cria um GameObject novo, com o nome passado em mapName.
        Transform mapHolder = new GameObject(mapName).transform;

        //Torna o objeto criado filho do objeto que contém este script (iguala o 'transform' dele com o deste objeto).
        mapHolder.parent = transform;

        //Normalizadores

        int mapCenter = 0;
        float offset = 0;


        if (centralized)
        {
            mapCenter = 2;
            offset = 0.5f;
        }
        else
        {
            mapCenter = 1;
            offset = 1f;
        }

        //Percorre o eixo X da matriz.
        for (int x = 0; x < mapSize.x; x++)
        {
            //Percorre o eixo Z da matriz.
            for (int z = 0; z < mapSize.z; z++)
            {
                //Armazena a posição em Vector3 de cada bloco do labirinto.
                Vector3 tilePosition = new Vector3(-mapSize.x / mapCenter + offset + x, 0f, -mapSize.z / mapCenter + offset + z);

                int localIndex = 0;

                //Se o valor for maior que o tamanho do vetor:
                if (matrixMap[x, z] < mazeBlocks.Length)
                {
                    localIndex = matrixMap[x, z];
                }
                else
                {
                    localIndex = 0;
                }

                Transform newBlock = Instantiate(mazeBlocks[localIndex], tilePosition, Quaternion.Euler(new Vector3(0f, rotationMap[x, z], 0f)));  //Quaternion.Euler(Vector3.up * rotationTo

                //Manter a escala em 100%.
                newBlock.localScale = Vector3.one;

                //"newBlock" recebe  o mesmo transform do pai ("mapHolder").
                newBlock.parent = mapHolder;
            }
        }
        mapHolder.localScale = Vector3.one * scaleTo;
    }
}