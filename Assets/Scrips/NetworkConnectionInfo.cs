using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkConnectionInfo
{
    //Host 실행여부
    public bool Host;

    //클라 실행시 호스트 주소
    public string IPAddress;

    //클라 실행시 포트
    public int Port;

}
