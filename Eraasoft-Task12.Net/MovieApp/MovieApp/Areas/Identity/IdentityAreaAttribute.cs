using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Areas.Identity
{
    [Area("Identity")]
    public class IdentityAreaAttribute : AreaAttribute
    {
        public IdentityAreaAttribute() : base("Identity")
        {
        }
    }
}