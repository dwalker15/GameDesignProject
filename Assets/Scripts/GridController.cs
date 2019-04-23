using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GridController : MonoBehaviour
{

    GameObject Zones;
    List<Lot> BuildingLocations = new List<Lot>();
    bool GameOver = false;

    public Button button;
  

    // Start is called before the first frame update
    void Start()
    {
        Grid grid = new Grid(5);
        grid.CreateBuildingPlan();
        grid.CreateZones();
        List<Tuple<bool, Lot, Lot>> barrierList = grid.PlaceBarriers();
        Zones = new GameObject();
        Zones.transform.position.Set(0, 0, 0);

        //List<Button> buttons = FindObjectsOfType<Button>().ToList();
        //Button startOverButton = buttons.ElementAt(0);
        //Button helpButton = buttons.ElementAt(1);
        //startOverButton.onClick.AddListener(() => {
        //    if (GameOver)
        //    {
        //        SceneManager.LoadScene(0);
        //        GameOver = false;
        //    }
        //    else
        //    {
        //        int index = 0;
        //        while (index < Zones.transform.childCount)
        //        {
        //            var zone = Zones.transform.GetChild(index++);
        //            int lotIndex = 0;
        //            while (lotIndex < zone.childCount)
        //            {
        //                var lot = zone.GetChild(lotIndex++);
        //                lot.GetChild(0).gameObject.SetActive(false);
        //                lot.GetChild(1).gameObject.SetActive(false);
        //                lot.GetChild(2).gameObject.SetActive(false);
        //            }

        //        }
        //    }
        //});
        GameObject RedZone = new GameObject("RedZone");
        GameObject BlueZone = new GameObject("BlueZone");
        GameObject YellowZone = new GameObject("YellowZone");
        GameObject GreenZone = new GameObject("GreenZone");
 

        foreach (var item in grid.lotList)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(0.2f, 1, 0.2f);
            int xPos = -4 + 2 * item.x;
            int zPos = -4 + 2 * item.y;
            plane.transform.position = new Vector3(xPos, 0, zPos);

            GameObject cone = (GameObject)Instantiate(Resources.Load("SA_TrafficCone_01"));
            cone.transform.position = plane.transform.position;
            cone.transform.eulerAngles = new Vector3(-90, 0, 0);
            cone.SetActive(false);
            cone.transform.SetParent(plane.transform);

            GameObject sign = (GameObject)Instantiate(Resources.Load("Hard_Hat_Sign"));
            sign.transform.position = new Vector3(xPos, 2, zPos);
            sign.transform.eulerAngles = new Vector3(-55, 180, 0);
            sign.SetActive(false);
            sign.transform.SetParent(plane.transform);

            GameObject pole = (GameObject)Instantiate(Resources.Load("Road_Sign_Pole"));
            pole.transform.position = new Vector3(xPos, 1, zPos);
            pole.transform.eulerAngles = new Vector3(-90, 0, -90);
            pole.SetActive(false);
            pole.transform.SetParent(plane.transform);
            //if (item.hasTree)
            //{
            //    Tuple<string, Vector3> buildingDetails = buildings.First();
            //    buildings.Remove(buildingDetails);
            //    GameObject building = (GameObject)Instantiate(Resources.Load(buildingDetails.Item1));
            //    building.transform.localScale = buildingDetails.Item2;
            //    float yPos = 0;
            //    if (buildingDetails.Item1 == "container_4_open")
            //    {
            //        yPos = 0.29f;
            //    }
            //    else if (buildingDetails.Item1 == "det_K_16")
            //    {
            //        yPos = 1.375f;
            //    }
            //    building.transform.position = new Vector3(xPos, yPos, zPos);
            //    building.SetActive(false);
            //    BuildingLocations.Add(item);
            //}
            List<Tuple<bool, Lot, Lot>> barriers = barrierList.Where(x => x.Item2 == item || x.Item3 == item).ToList();
            
            foreach (var barrier in barriers)
            {
                GameObject b = (GameObject)Instantiate(Resources.Load("TrafficBarrierWhite"));
                b.transform.localScale = new Vector3(1.6f, 1, 0.5f);
                if (!barrier.Item1)
                {
                    b.transform.eulerAngles = new Vector3(0, 90, 0);
                }
                if (barrier.Item2.x == barrier.Item3.x)
                {
                    b.transform.position = new Vector3((-4 + 2 *barrier.Item2.x), 0, barrier.Item2.y + barrier.Item3.y - 4);
                }
                else
                {
                    b.transform.position = new Vector3(barrier.Item2.x + barrier.Item3.x - 4, 0,(-4 + 2* barrier.Item2.y));
                }
            }

            switch (item.color)
            {
                case Lot.Color.None:
                    break;
                case Lot.Color.Red:
                    plane.transform.parent = RedZone.transform;
                    plane.GetComponent<Renderer>().material = (Material)Resources.Load("Dry ground pattern");
                    break;
                case Lot.Color.Yellow:
                    plane.transform.parent = YellowZone.transform;
                    plane.GetComponent<Renderer>().material = (Material)Resources.Load("Grass & rocks pattern");
                    break;
                case Lot.Color.Green:
                    plane.transform.parent = GreenZone.transform;
                    plane.GetComponent<Renderer>().material = (Material)Resources.Load("Ground & rocks pattern 02");
                    break;
                case Lot.Color.Blue:
                    plane.transform.parent = BlueZone.transform;
                    plane.GetComponent<Renderer>().material = (Material)Resources.Load("Ground pattern 01");
                    break;
                default:
                    break;
            }
        }

        RedZone.transform.parent = Zones.transform;
        YellowZone.transform.parent = Zones.transform;
        GreenZone.transform.parent = Zones.transform;
        BlueZone.transform.parent = Zones.transform;

        
    }

    bool CheckWin()
    {
        int index = 0;
        List<Vector3> locations = new List<Vector3>();
        List<Transform> lots = new List<Transform>();
        while (index < Zones.transform.childCount)
        {
            var zone = Zones.transform.GetChild(index++);
            int lotIndex = 0;
            int hasSign = 0;
            while (lotIndex < zone.childCount)
            {
                var lot = zone.GetChild(lotIndex++);
                if (lot.GetChild(1).gameObject.activeInHierarchy)
                {
                    hasSign++;
                    locations.Add(lot.position);
                    lots.Add(lot);
                }
            }
            if (hasSign != 1)
            {
                return false;
            }
        }

        foreach (var item in locations)
        {
            if (locations.Except(new List<Vector3>() { item }).Where(loc => (loc.x == item.x && loc.z != item.z) || // Same Col
                                                                            (loc.z == item.z && loc.x != item.x) || // Same Row
                                                                            (Math.Abs(loc.x - item.x) == 1 && Math.Abs(loc.z - item.z) == 1)).Any()) // Adjacent
            {
                return false;
            }
        }

        List<Tuple<string, Vector3>> buildings = new List<Tuple<string, Vector3>>
        {
            new Tuple<string, Vector3>("container_4_open", new Vector3(0.12f, 0.15f, 0.2f)),
            new Tuple<string, Vector3>("det_K_16", new Vector3(0.25f, 0.2f, 0.25f)),
            new Tuple<string, Vector3>("building_002", new Vector3(0.125f, 0.125f, 0.125f)),
            new Tuple<string, Vector3>("Large Tank", new Vector3(0.075f, 0.075f, 0.075f))

        };

        foreach (var item in locations)
        {
           
            Tuple<string, Vector3> buildingDetails = buildings.First();
            buildings.Remove(buildingDetails);
            GameObject building = (GameObject)Instantiate(Resources.Load(buildingDetails.Item1));
            building.transform.localScale = buildingDetails.Item2;
            float yPos = 0;
            if (buildingDetails.Item1 == "container_4_open")
            {
                yPos = 0.29f;
            }
            else if (buildingDetails.Item1 == "det_K_16")
            {
                yPos = 1.375f;
            }
            building.transform.position = new Vector3(item.x, yPos, item.z);
            building.SetActive(true);

            var buildLot = lots.Where(lot => lot.transform.position == item).FirstOrDefault();
            int lotIndex = 0;
            while (lotIndex < buildLot.childCount)
            {
                buildLot.GetChild(lotIndex++).gameObject.SetActive(false);
            }

        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && !GameOver)
        {
            if (Physics.Raycast(ray, out RaycastHit Hit))
            {
                if (Hit.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    Hit.transform.GetChild(0).gameObject.SetActive(false);
                    Hit.transform.GetChild(1).gameObject.SetActive(true);
                    Hit.transform.GetChild(2).gameObject.SetActive(true);
                }
                else if (Hit.transform.GetChild(1).gameObject.activeInHierarchy)
                {

                    Hit.transform.GetChild(0).gameObject.SetActive(false);
                    Hit.transform.GetChild(1).gameObject.SetActive(false);
                    Hit.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    Hit.transform.GetChild(0).gameObject.SetActive(true);
                    Hit.transform.GetChild(1).gameObject.SetActive(false);
                    Hit.transform.GetChild(2).gameObject.SetActive(false);
                }
            }

            if (CheckWin())
            {
                GameOver = true;
                Canvas c = FindObjectOfType<Canvas>();
                var text = c.transform.GetChild(1);
                text.gameObject.SetActive(true);
                //  winText.gameObject.SetActive(true);


                //Canvas canvasGO = FindObjectOfType<Canvas>();
                //canvasGO.renderMode = RenderMode.ScreenSpaceOverlay;

                //// Create the Text GameObject.
                //GameObject textGO = new GameObject();
                //textGO.transform.parent = canvasGO.transform;
                //textGO.AddComponent<Text>();

                //// Set Text component properties.
                //Text text = textGO.GetComponent<Text>();
                //text.text = "You Win!!!";
                //text.fontSize = 48;
                //text.alignment = TextAnchor.MiddleCenter;

                //// Provide Text position and size using RectTransform.
                //RectTransform rectTransform;
                //rectTransform = text.GetComponent<RectTransform>();
                //rectTransform.localPosition = new Vector3(0, 0, 0);
                //rectTransform.sizeDelta = new Vector2(600, 200);


            }
        }

    }

    public class Zone
    {
        public readonly int ZoneID;
        public readonly Lot.Color Color;
        public List<Lot> LotList;
        public List<Lot> Possibles;

        public Zone(int id, Lot.Color color)
        {
            ZoneID = id;
            Color = color;
            LotList = new List<Lot>();
            Possibles = new List<Lot>();
        }

    }

    public class Lot
    {
        public enum Color { None, Red, Yellow, Green, Blue };

        public readonly int x;
        public readonly int y;
        public bool isOpen;
        public bool hasTree;

        public Color color;

        public Lot(int col, int row)
        {
            x = col;
            y = row;
            isOpen = true;
            hasTree = false;
            color = Color.None;
        }
    }

    public class Grid
    {
        private readonly int _size;
        public Lot[][] lotGrid;
        public List<Lot> lotList;
        private System.Random random;


        public Grid(int size)
        {
            _size = size;
            lotGrid = new Lot[size][];
            lotList = new List<Lot>();
            for (int i = 0; i < size; i++)
            {
                lotGrid[i] = new Lot[size];
                for (int j = 0; j < size; j++)
                {
                    Lot space = new Lot(i, j);
                    lotGrid[i][j] = space;
                    lotList.Add(space);
                }
            }
            random = new System.Random();
        }

        public void CreateBuildingPlan()
        {
            int count = 4;
            Stack<Lot> lotStack = new Stack<Lot>();

            int index = 0;
            while (lotStack.Count < count)
            {
                List<Lot> openLots = lotList.Where(lot => lot.isOpen).ToList();
                if (openLots.Count == 0)
                {
                    Lot lotToOpen = lotStack.Pop();
                    lotList.Where(lot => lot.x == lotToOpen.x && lot.y == lotToOpen.y).FirstOrDefault().hasTree = false;
                    lotList.Where(lot => lot.x == lotToOpen.x && lot.y == lotToOpen.y).FirstOrDefault().isOpen = false;
                    OpenBorder(lotToOpen);
                }
                openLots = lotList.Where(lot => lot.isOpen).ToList();
                Lot randLot = openLots.ElementAt(random.Next(openLots.Count));

                if (DrawBorder(randLot).Count() != 0 || lotStack.Count() == count - 1)
                {
                    lotStack.Push(randLot);
                    lotList.Where(lot => lot.x == randLot.x && lot.y == randLot.y).FirstOrDefault().hasTree = true;
                }

                lotList.Where(lot => lot.x == randLot.x && lot.y == randLot.y).FirstOrDefault().isOpen = false;
            }

            int colorIndex = 1;
            foreach (var item in lotStack)
            {
                item.color = (Lot.Color)Enum.GetValues(typeof(Lot.Color)).GetValue(colorIndex++);
                item.hasTree = true;
            }
        }

        public List<Lot> DrawBorder(Lot randLot)
        {
            List<Lot> adjacentLots = lotList.Where(lot => lot.x == randLot.x).ToList()
                                                .Union(lotList.Where(lot => lot.y == randLot.y)).ToList()
                                                    .Union(lotList.Where(lot => Math.Abs(lot.x - randLot.x) == 1 && Math.Abs(lot.y - randLot.y) == 1)).ToList();


            List<Lot> closedLots = lotList.Where(lot => !lot.isOpen).ToList();
            List<Lot> openLots = lotList.Except(closedLots.Union(adjacentLots)).ToList();
            if (openLots.Count() == 0)
            {
                return new List<Lot>();
            }
            else
            {
                foreach (var item in adjacentLots)
                {
                    lotList.Where(lot => lot == item).FirstOrDefault().isOpen = false;
                }
                adjacentLots.Remove(adjacentLots.Where(lot => lot.x == randLot.x && lot.y == randLot.y).FirstOrDefault());
                return adjacentLots;
            }
        }

        public void OpenBorder(Lot randLot)
        {
            List<Lot> lots = lotList.Where(lot => !lot.hasTree).ToList();
            foreach (var item in lots)
            {
                lotList.Where(lot => lot.x == item.x && lot.y == item.y).FirstOrDefault().isOpen = true;
            }

            List<Lot> buildings = lotList.Where(lot => lot.hasTree).ToList();
            foreach (var item in buildings)
            {
                DrawBorder(item);
            }
        }

        public void CreateZones()
        {
            List<Lot> buildingList = lotList.Where(x => x.hasTree).ToList();
            List<Zone> zoneList = new List<Zone>();
            int id = 0;
            foreach (var item in buildingList)
            {
                Zone zone = new Zone(id, item.color);
                zone.LotList.Add(item);
                //   List<Space> adj = Zones.spaceList.Where(lot => (Math.Abs(lot.x - item.x) <= 1 && Math.Abs(lot.y - item.y) <= 1)).ToList();

                zone.Possibles.AddRange(lotList.Where(lot => ((Math.Abs(lot.x - item.x) <= 1 && lot.y == item.y) ||
                                                                     ((Math.Abs(lot.y - item.y) <= 1 && lot.x == item.x)) &&
                                                                     lot.color == Lot.Color.None)).ToList());
                zone.Possibles.Remove(item);
                zoneList.Add(zone);
            }

            while (lotList.Where(x => x.color == Lot.Color.None).Any())
            {
                foreach (var item in zoneList)
                {
                    if (item.Possibles.Count == 0)
                    {
                        continue;
                    }
                    Lot s = item.Possibles.ElementAt(random.Next(item.Possibles.Count));
                    s.color = item.Color;
                    List<Zone> otherAreas = zoneList.Except(new List<Zone>() { item }).ToList();
                    foreach (var area in otherAreas)
                    {
                        area.Possibles.Remove(s);
                    }
                    zoneList.Where(x => x == item).FirstOrDefault().LotList.Add(s);
                    item.Possibles.AddRange(lotList.Where(lot => ((Math.Abs(lot.x - s.x) <= 1 && lot.y == s.y) ||
                                                                         ((Math.Abs(lot.y - s.y) <= 1 && lot.x == s.x)) &&
                                                                         lot.color == Lot.Color.None)).ToList());
                    item.Possibles = item.Possibles.Where(x => !x.hasTree && x.color == Lot.Color.None).Distinct().ToList();
                }
            }
        }

        internal List<Tuple<bool, Lot, Lot>> PlaceBarriers()
        {
            List<Tuple<bool, Lot, Lot>> barrierList = new List<Tuple<bool, Lot, Lot>>();
            foreach (var item in lotList)
            {
                List<Lot> lonNeighbors = lotList.Where(lot => (Math.Abs(lot.x - item.x) == 1 && Math.Abs(lot.y - item.y) == 0)).ToList();
                List<Lot> latNeighbors = lotList.Where(lot => (Math.Abs(lot.x - item.x) == 0 && Math.Abs(lot.y - item.y) == 1)).ToList();
                foreach (var lot in lonNeighbors)
                {
                    if ((lot.color != item.color) && (!barrierList.Where(b => b.Item2 == lot && b.Item3 == item).Any()))
                    {
                        barrierList.Add(new Tuple<bool, Lot, Lot>(false, item, lot));
                    }
                }

                foreach (var lot in latNeighbors)
                {
                    if ((lot.color != item.color) && (!barrierList.Where(b => b.Item2 == lot && b.Item3 == item).Any()))
                    {
                        barrierList.Add(new Tuple<bool, Lot, Lot>(true, item, lot));
                    }
                }
            }

            return barrierList;
        }
    }
}
