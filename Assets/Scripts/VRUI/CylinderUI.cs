using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityVolumeRendering;
public class CylinderUI : MonoBehaviour
{
    enum CylinderPanels
    {
        LOAD_DATA_PANEL = 0,
        VOLUME_PROPOERTIES_PANEL,
        TRANSFER_FUNCTION_PANEL,
        SLICING_PANEL,
        CROSS_SECTION_PANEL,
        NEEDLING_PANEL,
        DRAW_PANEL,
        END_PANEL
    }
    public Transform DataPanel = null;
    public Transform VolumePropoertiesPanel;
    public Transform TransferFunctionPanel;
    public Transform SlicingPanel;
    public Transform CrossSectionPanel;
    public Transform NeedlingPanel;
    public Transform DrawingPanel;

    private bool[] mIsPanelOn;

    //public GameObject mTargetVolume = null;//  { get; private set; } = null;

    public VolumeRenderedObject mTargetVolume { get; private set; } = null;

    //public ref VolumeRenderedObject getTargetVolume()
    //{
    //    return ref mTargetVolume;
    //}
    private void Awake()
    {
        mIsPanelOn = Enumerable.Repeat(false, (int)CylinderPanels.END_PANEL).ToArray();
        VolumeRenderedObject.isSnapAble = true;

        //MENGHE:!WORK ON THAT TO ENABLE MORE OBJS!!!!
        try
        {
            mTargetVolume = GameObject.FindGameObjectWithTag("VolumeRenderingObject").GetComponent<VolumeRenderedObject>();
        }
        catch (Exception e)
        {
            Debug.Log("No Volume Object now in the scene: " + e.Message);
        }
    }
    private void change_panel_status(in GameObject obj, int pos)
    {
        mIsPanelOn[pos] = !mIsPanelOn[pos];
        obj.SetActive(mIsPanelOn[pos]);
    }
    public void OnChangeDataLoading()
    {
        if (!DataPanel) return;
        change_panel_status(DataPanel.gameObject,
            (int)CylinderPanels.LOAD_DATA_PANEL);
        
        //RuntimeFileBrowser.ShowOpenDirectoryDialog(OnOpenDICOMDatasetResult);
    }
    public void OnChangeVolumePropoerties()
    {
        if (!VolumePropoertiesPanel) return;
        change_panel_status(
            VolumePropoertiesPanel.gameObject,
            (int)CylinderPanels.VOLUME_PROPOERTIES_PANEL);
    }
    public void OnChangeTransferFunction()
    {
        if (!TransferFunctionPanel) return;
        change_panel_status(
            TransferFunctionPanel.gameObject,
            (int)CylinderPanels.TRANSFER_FUNCTION_PANEL);
    }
    public void OnChangeCrossSection()
    {
        if (!CrossSectionPanel) return;
        change_panel_status(
            CrossSectionPanel.gameObject,
            (int)CylinderPanels.CROSS_SECTION_PANEL);
    }
    public void OnChangeSliceRendering()
    {
        if (!SlicingPanel) return;
        change_panel_status(
            SlicingPanel.gameObject,
            (int)CylinderPanels.SLICING_PANEL);
    }
    public void OnChangeNeedle()
    {
        if (!NeedlingPanel) return;
        change_panel_status(
            NeedlingPanel.gameObject,
            (int)CylinderPanels.NEEDLING_PANEL);
        try
        {
            NeedlingPanel.GetComponent<NeedlingEdit>().OnChangePanelStatus(mIsPanelOn[(int)CylinderPanels.NEEDLING_PANEL]);
        }catch(Exception e)
        {
            //do nothing
        }

    }
    public void OnChangeDrawing()
    {
        if (!DrawingPanel) return;
        change_panel_status(
            DrawingPanel.gameObject,
            (int)CylinderPanels.DRAW_PANEL);
    }
    private void OnOpenDICOMDatasetResult(RuntimeFileBrowser.DialogResult result)
    {
        Debug.LogWarning("=====load data back========");

        //    if (!result.cancelled)
        //    {
        //        // We'll only allow one dataset at a time in the runtime GUI (for simplicity)
        //        //DespawnAllDatasets();

        //        bool recursive = true;

        //        // Read all files
        //        IEnumerable<string> fileCandidates = Directory.EnumerateFiles(result.path, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
        //            .Where(p => p.EndsWith(".dcm", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicom", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicm", StringComparison.InvariantCultureIgnoreCase));

        //        // Import the dataset
        //        IImageSequenceImporter importer = ImporterFactory.CreateImageSequenceImporter(ImageSequenceFormat.DICOM);
        //        IEnumerable<IImageSequenceSeries> seriesList = importer.LoadSeries(fileCandidates);
        //        float numVolumesCreated = 0;
        //        foreach (IImageSequenceSeries series in seriesList)
        //        {
        //            VolumeDataset dataset = importer.ImportSeries(series);
        //            // Spawn the object
        //            if (dataset != null)
        //            {
        //                VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
        //                obj.transform.position = new Vector3(numVolumesCreated, 0, 0);
        //                numVolumesCreated++;
        //            }
        //        }
        //    }
    }
    //private void DespawnAllDatasets()
    //{
    //    VolumeRenderedObject[] volobjs = FindObjectsOfType<VolumeRenderedObject>();
    //    foreach (VolumeRenderedObject volobj in volobjs)
    //    {
    //        GameObject.Destroy(volobj.gameObject);
    //    }
    //}
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
