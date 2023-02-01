using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore
{
    class UserFunctions
    {
        //VerifyUserPermission - Check if current user has enough Permission level
        public static bool VerifyUserPermission(int clearanceLevel)
        {
            if (DBConn.instance.currentUser.permission >= clearanceLevel)
            {
                return true; //User has equal or higher permission level
            }
            else
            {
                return false; //User has lower permission level
            }
        }
    }
}
