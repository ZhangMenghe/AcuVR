using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnityVolumeRendering
{
    public class VolumeRendererEditorFunctions
    {
        [MenuItem("Volume Rendering/Load dataset/Load raw dataset")]
        static void ShowDatasetImporter()
        {
            string file = EditorUtility.OpenFilePanel("Select a dataset to load", "DataFiles", "");
            if (File.Exists(file))
            {
                RAWDatasetImporterEditorWindow wnd = (RAWDatasetImporterEditorWindow)EditorWindow.GetWindow(typeof(RAWDatasetImporterEditorWindow));
                if (wnd != null)
                    wnd.Close();

                wnd = new RAWDatasetImporterEditorWindow(file);
                wnd.Show();
            }
            else
            {
                Debug.LogError("File doesn't exist: " + file);
            }
        }

        [MenuItem("Volume Rendering/Load dataset/Load DICOM")]
        static void ShowDICOMImporter()
        {
            string dir = EditorUtility.OpenFolderPanel("Select a folder to load", "", "");
            if (Directory.Exists(dir))
            {
                bool recursive = true;

                // Read all files
                IEnumerable<string> fileCandidates = Directory.EnumerateFiles(dir, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(p => p.EndsWith(".dcm", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicom", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicm", StringComparison.InvariantCultureIgnoreCase));

                if (!fileCandidates.Any())
                {
#if UNITY_EDITOR
                    if (UnityEditor.EditorUtility.DisplayDialog("Could not find any DICOM files",
                        $"Failed to find any files with DICOM file extension.{Environment.NewLine}Do you want to include files without DICOM file extension?", "Yes", "No"))
                    {
                        fileCandidates = Directory.EnumerateFiles(dir, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    }
#endif
                }

                if (fileCandidates.Any())
                {
                    IImageSequenceImporter importer = ImporterFactory.CreateImageSequenceImporter(ImageSequenceFormat.DICOM);
                    IEnumerable<IImageSequenceSeries> seriesList = importer.LoadSeries(fileCandidates);
                    float numVolumesCreated = 0;
                    string[] split_names = dir.Split('/');
                    string dataset_name = split_names[split_names.Length - 1];
                    foreach (IImageSequenceSeries series in seriesList)
                    {
                        VolumeDataset dataset = importer.ImportSeries(series);
                        if (dataset != null)
                        {
                            dataset.datasetName = dataset_name + (numVolumesCreated>0? "_"+numVolumesCreated.ToString():"");
                            if (EditorPrefs.GetBool("DownscaleDatasetPrompt"))
                            {
                                if (EditorUtility.DisplayDialog("Optional DownScaling",
                                    $"Do you want to downscale the dataset? The dataset's dimension is: {dataset.dimX} x {dataset.dimY} x {dataset.dimZ}", "Yes", "No"))
                                {
                                    dataset.DownScaleData();
                                }
                            }
                            if(seriesList.Count() == 1)
                            {
                                string[] mask_dirs = Directory.GetDirectories(dir, "mask");
                                if (mask_dirs.Length > 0)
                                {
                                    IEnumerable<string> maskCandidates = Directory.EnumerateFiles(mask_dirs[0], "*.*", SearchOption.TopDirectoryOnly)
                                    .Where(p => p.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase));
                                    if (maskCandidates.Count() == dataset.dimZ)
                                    {
                                        dataset.maskTexture = new Texture3D(dataset.dimX, dataset.dimY, dataset.dimZ, TextureFormat.R8, false);
                                        dataset.maskTexture.wrapMode = TextureWrapMode.Clamp;
                                        int single_length = dataset.dimX * dataset.dimY;
                                        int data_length = single_length * dataset.dimZ;
                                        bool isHalfFloat = false;
                                        int sampleSize = 1;// isHalfFloat ? 2 : 4;
                                        byte[] flat_data = new byte[data_length * sampleSize];
                                        for (int i = 0, zoffset=0; i < maskCandidates.Count(); i++, zoffset+= single_length)
                                        {
                                            Texture2D tex = new Texture2D(dataset.dimX, dataset.dimY);
                                            tex.LoadImage(File.ReadAllBytes(mask_dirs[0] + "/" + i.ToString() + ".png"));

                                            Color[] raw_pixels = tex.GetPixels();

                                            for (int iData = 0; iData < single_length; iData++)
                                            {
                                                int value = (int)(raw_pixels[iData].r * 255);
                                                flat_data[zoffset + iData] = BitConverter.GetBytes(value)[0];
                                            }
                                        }
                                        dataset.maskTexture.SetPixelData(flat_data, 0);
                                        dataset.maskTexture.Apply();
                                        //debug
                                        // Save the texture to your Unity Project
                                        //AssetDatabase.CreateAsset(dataset.maskTexture, "Assets/3d_mask_texture.asset");
                                    }
                                }
                            }

                            
                            VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                            //obj.transform.position = new Vector3(numVolumesCreated, 0, 0);
                            numVolumesCreated++;
                        }
                    }

                    //Try to load Mask
                }    
                else
                    Debug.LogError("Could not find any DICOM files to import.");

            }
            else
            {
                Debug.LogError("Directory doesn't exist: " + dir);
            }
        }

        [MenuItem("Volume Rendering/Load dataset/Load NRRD dataset")]
        static void ShowNRRDDatasetImporter()
        {
            if (!SimpleITKManager.IsSITKEnabled())
            {
                if (EditorUtility.DisplayDialog("Missing SimpleITK", "You need to download SimpleITK to load NRRD datasets from the import settings menu.\n" +
                    "Do you want to open the import settings menu?", "Yes", "No"))
                {
                    ImportSettingsEditorWindow.ShowWindow();
                }
                return;
            }

            string file = EditorUtility.OpenFilePanel("Select a dataset to load (.nrrd)", "DataFiles", "");
            if (File.Exists(file))
            {
                IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NRRD);
                VolumeDataset dataset = importer.Import(file);

                if (dataset != null)
                {
                    VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                }
                else
                {
                    Debug.LogError("Failed to import datset");
                }
            }
            else
            {
                Debug.LogError("File doesn't exist: " + file);
            }
        }

        [MenuItem("Volume Rendering/Load dataset/Load NIFTI dataset")]
        static void ShowNIFTIDatasetImporter()
        {
            if (!SimpleITKManager.IsSITKEnabled())
            {
                if (EditorUtility.DisplayDialog("Missing SimpleITK", "You need to download SimpleITK to load NRRD datasets from the import settings menu.\n" +
                    "Do you want to open the import settings menu?", "Yes", "No"))
                {
                    ImportSettingsEditorWindow.ShowWindow();
                }
                return;
            }

            string file = EditorUtility.OpenFilePanel("Select a dataset to load (.nii)", "DataFiles", "");
            if (File.Exists(file))
            {
                IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
                VolumeDataset dataset = importer.Import(file);

                if (dataset != null)
                {
                    VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                }
                else
                {
                    Debug.LogError("Failed to import datset");
                }
            }
            else
            {
                Debug.LogError("File doesn't exist: " + file);
            }
        }

        [MenuItem("Volume Rendering/Load dataset/Load PARCHG dataset")]
        static void ShowParDatasetImporter()
        {
            string file = EditorUtility.OpenFilePanel("Select a dataset to load", "DataFiles", "");
            if (File.Exists(file))
            {
                IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.VASP);
                VolumeDataset dataset = importer.Import(file);

                if (dataset != null)
                {
                    VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                }
                else
                {
                    Debug.LogError("Failed to import datset");
                }
            }
            else
            {
                Debug.LogError("File doesn't exist: " + file);
            }
        }

        [MenuItem("Volume Rendering/Load dataset/Load image sequence")]
        static void ShowSequenceImporter()
        {
            string dir = EditorUtility.OpenFolderPanel("Select a folder to load", "", "");
            
            if (Directory.Exists(dir))
            {
                List<string> filePaths = Directory.GetFiles(dir).ToList();
                IImageSequenceImporter importer = ImporterFactory.CreateImageSequenceImporter(ImageSequenceFormat.ImageSequence);

                IEnumerable<IImageSequenceSeries> seriesList = importer.LoadSeries(filePaths);

                foreach(IImageSequenceSeries series in seriesList)
                {
                    VolumeDataset dataset = importer.ImportSeries(series);
                    if (dataset != null)
                    {
                        if (EditorPrefs.GetBool("DownscaleDatasetPrompt"))
                        {
                            if (EditorUtility.DisplayDialog("Optional DownScaling",
                                $"Do you want to downscale the dataset? The dataset's dimension is: {dataset.dimX} x {dataset.dimY} x {dataset.dimZ}", "Yes", "No"))
                            {
                                dataset.DownScaleData();
                            }
                        }
                        VolumeObjectFactory.CreateObject(dataset);
                    }
                }
            }
            else
            {
                Debug.LogError("Directory doesn't exist: " + dir);
            }
        }

        [MenuItem("Volume Rendering/Cross section/Cross section plane")]
        static void OnMenuItemClick()
        {
            VolumeRenderedObject[] objects = GameObject.FindObjectsOfType<VolumeRenderedObject>();
            if (objects.Length == 1)
                objects[0].CreateCrossSectionPlane();
            else
            {
                CrossSectionPlaneEditorWindow wnd = new CrossSectionPlaneEditorWindow();
                wnd.Show();
            }
        }

        [MenuItem("Volume Rendering/Cross section/Box cutout")]
        static void SpawnCutoutBox()
        {
            VolumeRenderedObject[] objects = GameObject.FindObjectsOfType<VolumeRenderedObject>();
            if (objects.Length == 1)
                VolumeObjectFactory.SpawnCutoutBox(objects[0]);
        }

        [MenuItem("Volume Rendering/1D Transfer Function")]
        public static void Show1DTFWindow()
        {
            VolumeRenderedObject volRendObj = SelectionHelper.GetSelectedVolumeObject();
            if (volRendObj != null)
            {
                volRendObj.SetTransferFunctionMode(TFRenderMode.TF1D);
                TransferFunctionEditorWindow.ShowWindow(volRendObj);
            }
            else
            {
                EditorUtility.DisplayDialog("No imported dataset", "You need to import a dataset first", "Ok");
            }
        }

        [MenuItem("Volume Rendering/2D Transfer Function")]
        public static void Show2DTFWindow()
        {
            TransferFunction2DEditorWindow.ShowWindow();
        }

        [MenuItem("Volume Rendering/Slice renderer")]
        static void ShowSliceRenderer()
        {
            //DisplayRackFactory.AttachToRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, "Slicing Planes");
            //VolumeRenderedObject[] objects = GameObject.FindObjectsOfType<VolumeRenderedObject>();
            ////if (objects.Length == 1)
            //{
            //    objects[0].CreateSlicingPlane();
            //}
            //else
            //{
                SlicingPlaneEditorWindow wnd = new SlicingPlaneEditorWindow();
                wnd.Show();
            //}
            //SliceRenderingEditorWindow.ShowWindow();
        }
        [MenuItem("Volume Rendering/Add Needle")]
        static void AddAcuNeedle()
        {
            VolumeRenderedObject[] objects = GameObject.FindObjectsOfType<VolumeRenderedObject>();
            if (objects.Length == 1)
            {
                //MENGHE: MANAGE THE NEEDLES SOMEWHERE ELSE
                GameObject acuNeedleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/acuNeedlePrefab"));
                acuNeedleObj.name = "AcuNeedle";
                acuNeedleObj.GetComponent<AcuNeedle>().Initialize(objects[0]);
            }
                //objects[0].CreateSlicingPlane();
        }


        [MenuItem("Volume Rendering/Settigs")]
        static void ShowSettingsWindow()
        {
            ImportSettingsEditorWindow.ShowWindow();
        }
    }
}
