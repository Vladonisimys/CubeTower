using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePosition nowCube = new CubePosition(0, 1, 0);
    private Rigidbody allCubesRb;

    public float cubeChangeCubeSpeed = 0.5f;
    private float camMoveToYPosition = 0, camMoveSpeed = 2f;
    private int prevCountMaxHorizontal = 0;

    private bool IsLose, firstCube;

    public Transform cubeToPlace;
    private Transform mainCam;

    private Coroutine showCubePlace;

    public Text scoreTxt;

    public GameObject[] cubesToCreate;
    public GameObject allCubes, vfx;
    public GameObject[] canvasStartPage;
    public GameObject restartButton;

    public Color[] bgColors;
    private Color toCameraColor;

    private List<GameObject> isEnabledToCreate = new List<GameObject>();
    private List<Vector3> allCubesPositions = new List<Vector3>
    {
    new Vector3(0,0,0),
    new Vector3(0,1,0),
    new Vector3(1,0,0),
    new Vector3(-1,0,0),
    new Vector3(0,0,1),
    new Vector3(0,0,-1),
    new Vector3(1,0,1),
    new Vector3(1,0,-1),
    new Vector3(-1,0,1),
    new Vector3(-1,0,-1),
    };



    private void Start()
    {
        AddPosibleCubes(PlayerPrefs.GetInt("score"));
        scoreTxt.text = $"<size=36>Best:  {PlayerPrefs.GetInt("score")} </size>\n<size=26><color=#802C41> now: </color></size><size=28><color=#802C41> 0</color></size>";
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.45f + nowCube.y - 1f;
        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());

    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
/*#if !UNITY_EDITOR
      if(Input.GetTouch(0).phase != TouchPhase.Began)
      return;
#endif
*/

            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
            }

            GameObject createCube = null;
            if (isEnabledToCreate.Count == 1)
                createCube = isEnabledToCreate[0]; 
            else
                createCube = isEnabledToCreate[UnityEngine.Random.Range(0, isEnabledToCreate.Count)];

            GameObject newCube = Instantiate(createCube, cubeToPlace.position, Quaternion.identity) as GameObject;

            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(newCube.transform.position);
            allCubesPositions.Add(nowCube.getVector());

            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play();

            GameObject newVfx = Instantiate(vfx, newCube.transform.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, 2.5f);

            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;

            SpawnPositions();
            MoveCameraChangeBg();
        }

        if (!IsLose && allCubesRb.velocity.magnitude >= 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);
            restartButton.SetActive(true);
        }


        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),
            camMoveSpeed * Time.deltaTime);

        if (Camera.main.backgroundColor != toCameraColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
    }
    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(cubeChangeCubeSpeed);
        }
    }

    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0)
            return false;

        foreach (Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
                return false;
        }
        return true;
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor = 0;

        foreach (Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);

            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }

        if (PlayerPrefs.GetInt("score") < maxY)
            PlayerPrefs.SetInt("score", maxY);
        scoreTxt.text = $"<size=36>Best:  {PlayerPrefs.GetInt("score")} </size>\n<size=26><color=#802C41> now: </color></size><size=28><color=#802C41> {maxY}</color></size>";

        camMoveToYPosition = 5.45f + nowCube.y - 1f;
        maxHor = maxX > maxZ ? maxX : maxZ;

        if (maxHor % 3 == 0 && prevCountMaxHorizontal != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 2.5f);
            prevCountMaxHorizontal = maxHor;
        }

        if (maxY >= 9)
            toCameraColor = bgColors[2];
        else if (maxY >= 6)
            toCameraColor = bgColors[1];
        else if (maxY >= 3)
            toCameraColor = bgColors[0];
    }

    private void AddPosibleCubes(int best)
    {
        int[] scoresToUnlock = { 4, 8, 14, 16, 22, 28, 38, 50, 65 };
        int i = 0;        
        foreach ( int score in scoresToUnlock)
        {
            if (best >= score)
            {
                i++;
                isEnabledToCreate.Add(cubesToCreate[i]);
            }
            else
            {
                isEnabledToCreate.Add(cubesToCreate[0]);
                break;
            }
        }
    }
}

struct CubePosition
{
    public int x, y, z;

    public CubePosition(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 position)
    {
        x = Convert.ToInt32(position.x);
        y = Convert.ToInt32(position.y);
        z = Convert.ToInt32(position.z);
    }
}