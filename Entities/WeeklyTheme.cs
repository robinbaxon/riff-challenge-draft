using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace riff_challenge_draft.Entities
{
    public class WeeklyTheme
    {
        public string Name { get; set; }
        public bool IsWildcard { get
            {
                return new Random().Next(0, 1) > 0;
            }
        }

    }
}
