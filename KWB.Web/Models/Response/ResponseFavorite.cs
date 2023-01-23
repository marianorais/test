using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseFavorite
    {
        public int FavoriteID { get; set; }
        public int? PlaceID { get; set; }
        public DateTime? Fecha { get; set; }
        public int? UserID { get; set; }
        public List<Favorite>? FavoritesDB { get; set; }

    }
}
