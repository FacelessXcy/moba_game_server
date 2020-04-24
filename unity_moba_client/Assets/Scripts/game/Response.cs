using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Response
{
    public const int OK = 1;
    
    public const int SystemErr = -100;
    public const int UserIsFreeze = -101;
    public const int UserIsNotGuest = -102;
    public const int InvalidParams = -103;
    public const int UnameIsExist = -104;
    public const int UnameOrUpwdError = -105;

    public const int InvalidOpt = -106;
}
