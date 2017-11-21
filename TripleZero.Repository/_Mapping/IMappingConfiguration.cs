using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository._Mapping
{
    public interface IMappingConfiguration
    {
        IMapper GetConfigureMapper();
    }
}
