﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimecardApp.Model.Timelog
{
    public enum TimelogState
    {
        None,
        NoValidToken,
        ValidToken,
        UnexpectedError,
        ExectionSuccessfull,
        Running               
    }
}
