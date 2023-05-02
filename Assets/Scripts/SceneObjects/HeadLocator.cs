using UnityEngine;

public class HeadLocator:MonoBehaviour
{
    public Transform TopLocator;
    public Transform EarRightLocator;
    public Transform EarLeftLocator;
    public Transform EyeMidLocator;
    public Transform ScalpFrontLocator;
    public Transform ScalpBackLocator;

    public Transform LocatorCube;

    //private Vector3 mBBoxMax;
    //private Vector3 mBBoxMin;

    [HideInInspector]
    public Vector3 worldSize { get; private set; }

    [HideInInspector]
    public Vector3 mWorldSizeInv{ get; private set; }

    //[HideInInspector]
    //public Vector3 MidLocatorWorldPos { get; private set; }

    private GameObject refObj;
    private GameObject Obj;
    //[HideInInspector]
    //public BoxCollider mCollider { get; private set; }


    public void Initialize()
    {
        //mCollider = gameObject.AddComponent<BoxCollider>();

        Vector3 mBBoxMaxLocal = new Vector3(EarRightLocator.localPosition.x, TopLocator.localPosition.y, ScalpFrontLocator.localPosition.z);
        Vector3 mBBoxMinLocal = new Vector3(EarLeftLocator.localPosition.x, EyeMidLocator.localPosition.y, ScalpBackLocator.localPosition.z);

        LocatorCube.localPosition = (mBBoxMaxLocal + mBBoxMinLocal) * 0.5f;
        LocatorCube.localScale = mBBoxMaxLocal - mBBoxMinLocal;
        //mCollider.center = (mBBoxMaxLocal + mBBoxMinLocal) * 0.5f;
        //mCollider.size = mBBoxMaxLocal - mBBoxMinLocal;

        UpdateWorldPosition();
    }
    public void CreateVirtualMidObjs()
    {
        refObj = new GameObject();
        Obj = new GameObject();
    }

    public Vector3 GetRealSize(Vector3 refSize)
    {
        return new Vector3(
           refSize.x * mWorldSizeInv.x,
           refSize.y * mWorldSizeInv.y,
           refSize.z * mWorldSizeInv.z);
    }
    public void ResetScale()
    {
        transform.localScale = Vector3.one;

    }
    //private void UpdateLocalPosition()
    //{
    //    mBBoxMaxLocal = new Vector3(EarRightLocator.localPosition.x, HeadTopLocator.localPosition.y, ScalpFrontLocator.localPosition.z);
    //    mBBoxMinLocal = new Vector3(EarLeftLocator.localPosition.x, EyeMidLocator.localPosition.y, ScalpBackLocator.localPosition.z);
    //    mMidLocatorLocalPos = (mBBoxMaxLocal + mBBoxMinLocal) * 0.5f;
    //}

    public void UpdateWorldPosition()
    {
        //mBBoxMax = new Vector3(EarRightLocator.position.x, HeadTopLocator.position.y, ScalpFrontLocator.position.z);
        //mBBoxMin = new Vector3(EarLeftLocator.position.x, EyeMidLocator.position.y, ScalpBackLocator.position.z);
        //MidLocatorWorldPos = (mBBoxMax + mBBoxMin) * 0.5f;

        //worldSize = new Vector3(
        //Mathf.Abs(mBBoxMax.x - mBBoxMin.x),
        //Mathf.Abs(mBBoxMax.y - mBBoxMin.y),
        //Mathf.Abs(mBBoxMax.z - mBBoxMin.z));

        worldSize = LocatorCube.lossyScale;
        //worldSize = mCollider.bounds.size;
        mWorldSizeInv = new Vector3(1.0f / worldSize.x, 1.0f / worldSize.y, 1.0f / worldSize.z);
    }

    public void AlignLocators(Transform parentTransform, in HeadLocator refLocator)
    {
        Vector3 LocatorPosInRef = refLocator.LocatorCube.position;
        //Vector3 LocatorPosInRef = refLocator.MidLocatorWorldPos;
        //Vector3 LocatorPosInRef = refLocator.mCollider.bounds.center;
        Quaternion LocatorRotInRef = refLocator.transform.rotation;

        Vector3 LocatorPos = transform.localPosition;
        Quaternion LocatorRot = transform.localRotation;

        //CORE STEP!
        transform.position += LocatorPosInRef - LocatorCube.position;
        //transform.position += LocatorPosInRef - MidLocatorWorldPos;
        //transform.position += LocatorPosInRef - mCollider.bounds.center;
        transform.rotation = LocatorRotInRef;

        Vector3 offsetPosition = transform.localPosition - LocatorPos;
        Quaternion offsetRotation = Quaternion.Inverse(LocatorRot) * transform.localRotation;

        parentTransform.position += parentTransform.rotation * offsetPosition;
        parentTransform.rotation *= offsetRotation;

        transform.localPosition = LocatorPos;
        transform.localRotation = LocatorRot;

        UpdateWorldPosition();
    }


    //public Vector3 GetAlignedPosition_old(in HeadLocator refLocator, Vector3 needlePos)
    //{
    //    refLocator.UpdateWorldPosition();
    //    UpdateWorldPosition();
    //    var refMin = refLocator.mBBoxMin;
    //    var refWorld = refLocator.mWorldSizeInv;

    //    return new Vector3(
    //        (needlePos.x - refMin.x) * refWorld.x * worldSize.x,
    //        (needlePos.y - refMin.y) * refWorld.y * worldSize.y,
    //        (needlePos.z - refMin.z) * refWorld.z * worldSize.z) + mBBoxMin;
    //}

    public Vector3 GetAlignedPosition(in HeadLocator refLocator, Vector3 needlePos)
    {
        refLocator.UpdateWorldPosition();
        UpdateWorldPosition();

        //return VirtualMiddleLocator.TransformPoint(refLocator.VirtualMiddleLocator.InverseTransformPoint(needlePos));

        refObj.transform.position = refLocator.LocatorCube.position;
        //refObj.transform.position = refLocator.MidLocatorWorldPos;
        //refObj.transform.position = refLocator.mCollider.bounds.center;
        refObj.transform.rotation = refLocator.transform.rotation;
        var refNeedlePos = refObj.transform.InverseTransformPoint(needlePos);

        Obj.transform.position = LocatorCube.position;
        //Obj.transform.position = mCollider.bounds.center;
        //Obj.transform.position = MidLocatorWorldPos;
        Obj.transform.rotation = transform.rotation;
        return Obj.transform.TransformPoint(refNeedlePos);

        //GameObject.Destroy(refObj);
        //GameObject.Destroy(Obj);
        //return res;
        //var refNeedlePos = refLocator.transform.InverseTransformPoint(needlePos);
        //return transform.TransformDirection(refNeedlePos) ;
        //refLocator.UpdateWorldPosition();
        //UpdateWorldPosition();
        //var refMin = refLocator.mBBoxMin;
        //var refWorld = refLocator.mWorldSizeInv;

        //return new Vector3(
        //    (needlePos.x - refMin.x) * refWorld.x * worldSize.x,
        //    (needlePos.y - refMin.y) * refWorld.y * worldSize.y,
        //    (needlePos.z - refMin.z) * refWorld.z * worldSize.z) + mBBoxMin;
    }
}
