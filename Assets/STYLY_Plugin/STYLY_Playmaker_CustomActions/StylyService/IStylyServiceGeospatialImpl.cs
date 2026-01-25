using System;

namespace STYLY
{
    public interface IStylyServiceGeospatialImpl
    {
        void StreetscapeGeometryInit(Action<Exception> onFinished);
    }
}
