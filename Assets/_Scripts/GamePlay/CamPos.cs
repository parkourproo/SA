using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CamPos : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GridManager.Instance.doneLevelData != "");
        // tạo một vector3 để điều chỉnh vị trí camera
        Vector3 adjustment = new Vector3();
        // tính toán góc nhìn theo chiều ngang của camera
        float verticalFOV = cam.fieldOfView;
        float aspectRatio = cam.aspect;
        float horizontalFOV = 2 * Mathf.Atan(Mathf.Tan(verticalFOV * Mathf.Deg2Rad / 2) * aspectRatio) * Mathf.Rad2Deg;
        // dựa theo số cột của grid và góc nhìn chiều ngang của cam
        // để tăng chiều cao của camera (giải bài hình, tam giác cân có 1 góc là horizontalFOV độ, cạnh
        // đáy dài col + 1.5 + 1.5 (mỗi bên 1.5 để có thể nhìn 2 bên của xe bus))
        int col = GridManager.Instance.GetGridCol();
        float raiseHeight = Mathf.Tan((90 - horizontalFOV / 2) * Mathf.Deg2Rad) * ((float)col / 2 + 1.5f);
        //Debug.Log(raiseHeight);
        yield return new WaitUntil(() => BusManager.Instance.doneAllBusPosition != "");
        adjustment = BusManager.Instance.busStopPosition;
        adjustment.y += raiseHeight;

        // nâng cam lên 1 tí và sang phải 1 tí
        adjustment.y += raiseHeight * 0.05f;
        adjustment.x += 0.3f;
        cam.transform.position = adjustment;

    }

}
