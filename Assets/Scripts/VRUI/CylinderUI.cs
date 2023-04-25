using System.Linq;
using UnityEngine;

public class CylinderUI : MonoBehaviour
{
    enum CylinderPanels
    {
        //LOAD_DATA_PANEL = 0,
        VOLUME_PROPOERTIES_PANEL = 0,
        TRANSFER_FUNCTION_PANEL,
        SLICING_PANEL,
        //CROSS_SECTION_PANEL,
        NEEDLING_PANEL,
        WORKING_TABLE_PANEL,
        //DRAW_PANEL,
        END_PANEL
    }
    //public Transform DataPanel = null;
    public Transform VolumePropoertiesPanel;
    public Transform TransferFunctionPanel;
    public Transform SlicingPanel;
    //public Transform CrossSectionPanel;
    public Transform NeedlingPanel;
    //public Transform WorkingTable;

    //[HideInInspector]
    //public static bool UseHand = true;
    //public Transform DrawingPanel;

    private bool[] mIsPanelOn;
    private void Awake()
    {
        VolumeObjectFactory.SetUIManager(this);
        VolumeObjectFactory.GatherObjectsInScene();
        mIsPanelOn = Enumerable.Repeat(false, (int)CylinderPanels.END_PANEL).ToArray();
        StandardModelFactory.setCrossSectionManager(SlicingPanel.GetComponentInChildren<CrossSectionEdit>());
    }
    private void change_panel_status(in GameObject obj, int pos)
    {
        mIsPanelOn[pos] = !mIsPanelOn[pos];
        obj.SetActive(mIsPanelOn[pos]);
    }
    //public void OnChangeDataLoading()
    //{
    //    if (!DataPanel) return;
    //    change_panel_status(DataPanel.gameObject,
    //        (int)CylinderPanels.LOAD_DATA_PANEL);
        
    //    //RuntimeFileBrowser.ShowOpenDirectoryDialog(OnOpenDICOMDatasetResult);
    //}
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
    //public void OnChangeCrossSection()
    //{
    //    if (!CrossSectionPanel) return;
    //    change_panel_status(
    //        CrossSectionPanel.gameObject,
    //        (int)CylinderPanels.CROSS_SECTION_PANEL);
    //}
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
    }
    public void OnChangeStandardModel()
    {
        StandardModelFactory.OnChangeStandardModelStatus();
    }
}
