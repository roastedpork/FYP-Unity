using System;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class ScanManager : Scripts.Singleton<ScanManager> //, IInputClickHandler
{
    public TextMesh InstructionTextMesh;
    public Transform FloorPrefab;
    public Transform WallPrefab;
    public Transform SurfacePrefab;

    public GameObject SpatialMappingPrefab;
    public GameObject SpatialUnderstandingPrefab;

    private GameObject smHandler;
    private GameObject suHandler;



    public bool IsScanning { get; private set; }

    // Use this for initialization
    void Start()
    {
        IsScanning = false;
        InstructionTextMesh.text = "Say \"Begin scan\" to begin searching for floor depth";
    }
    
    public void StartScan()
    {
        if (HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance == null)
        {
            smHandler = Instantiate(SpatialMappingPrefab);
            smHandler.GetComponent<HoloToolkit.Unity.SpatialMapping.SpatialMappingManager>().DrawVisualMeshes = false;
            suHandler = Instantiate(SpatialUnderstandingPrefab);
            //suHandler.GetComponent<SpatialUnderstandingCustomMesh>().CreateMeshColliders = false;
            
            IsScanning = true;

            InstructionTextMesh.text = "Scanning for floor depth...";
        } 
    }

    public void StopScan()
    {
        if (IsScanning)
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
            HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance.StopObserver();
            Destroy(smHandler);
            Destroy(suHandler);
        }

        IsScanning = false;
        InstructionTextMesh.text = "";
    }
    

    // Update is called once per frame
    void Update()
    {
        if (IsScanning && GazeGestureManager.Instance.Focused)
        {
            Parameters.FloorDepth = (Parameters.FloorDepth > GazeGestureManager.Instance.position.y) ? GazeGestureManager.Instance.position.y : Parameters.FloorDepth;
            InstructionTextMesh.text = "Current Floor Depth: " + Parameters.FloorDepth.ToString(); 
        }




        /*
        switch (SpatialUnderstanding.Instance.ScanState)
        {
            case SpatialUnderstanding.ScanStates.None:
            case SpatialUnderstanding.ScanStates.ReadyToScan:
                break;
            case SpatialUnderstanding.ScanStates.Scanning:
                LogSurfaceState();
                break;
            case SpatialUnderstanding.ScanStates.Finishing:
                InstructionTextMesh.text = "State: Finishing Scan";
                break;
            case SpatialUnderstanding.ScanStates.Done:
                Vector3 origin = Camera.main.transform.position;
                Vector3 dir = Camera.main.transform.forward * GazeGestureManager.Instance.MaxRange;
                IntPtr resPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();

                int success = SpatialUnderstandingDll.Imports.PlayspaceRaycast(origin.x, origin.y, origin.z,
                                                                               dir.x, dir.y, dir.z,
                                                                               resPtr);

                SpatialUnderstandingDll.Imports.RaycastResult res = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();
                InstructionTextMesh.text = res.SurfaceType.ToString(); 
                break;
            default:
                break;
        }
        */
    }

    private void LogSurfaceState()
    {
        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) != 0)
        {
            var stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
            
            InstructionTextMesh.text = string.Format("TotalSurfaceArea: {0:0.##}\nWallSurfaceArea: {1:0.##}\nHorizSurfaceArea: {2:0.##}", stats.TotalSurfaceArea, stats.WallSurfaceArea, stats.HorizSurfaceArea);
        }
    }
    /*
    public void OnInputClicked(InputClickedEventData eventData)
    {
        InstructionTextMesh.text = "Requested Finish Scan";

        SpatialUnderstanding.Instance.RequestFinishScan();
    }
    
    private void InstanciateObjectOnFloor()
    {
        const int QueryResultMaxCount = 512;

        SpatialUnderstandingDllTopology.TopologyResult[] _resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];

        
        var minLengthFloorSpace = 0.25f;
        var minWidthFloorSpace = 0.25f;

        var resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(_resultsTopology);
        var locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(minLengthFloorSpace, minWidthFloorSpace, _resultsTopology.Length, resultsTopologyPtr);

        if (locationCount > 0)
        {
            Instantiate(FloorPrefab, _resultsTopology[0].position, Quaternion.LookRotation(_resultsTopology[0].normal, Vector3.up));

            InstructionTextMesh.text = "Placed the hologram";
        }
        else
        {
            InstructionTextMesh.text = "I can't found the enough space to place the hologram.";
        }
    }
    */
}