using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero._Mapping
{
    public interface IMappingConfiguration
    {
        IMapper GetConfigureMapper();
    }
}
