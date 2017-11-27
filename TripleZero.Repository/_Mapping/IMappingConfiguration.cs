using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository._Mapping
{
    internal interface IMappingConfiguration
    {
        IMapper GetConfigureMapper();
    }
}
