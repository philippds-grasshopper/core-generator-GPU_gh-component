using System;
using System.Timers;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using System.Web.Script.Serialization;
using Grasshopper;
using System.IdentityModel;
using System.Security;

using System.Threading;
using System.Threading.Tasks;

namespace core_generator
{
    public class generate_tower
    {
        //public List<Item> pjsonEnv = new List<Item>();
        int skin_width;
        int skin_height;
        int core_min_width;
        int core_min_height;
        double deviation;
        double core_area;
        
        public Rectangle3d skin;
        public List<Rectangle3d> cores;
        public List<Point3d> grid_pts = new List<Point3d>();
        public DataTree<bool> grid_val = new DataTree<bool>();

        public generate_tower(ref int sw, ref int sh, ref int cw, ref int ch, ref double e, ref double d)
        {
            // initialize values
            skin_width = sw;
            skin_height = sh;
            core_min_width = cw;
            core_min_height = ch;
            deviation = d;

            skin = new Rectangle3d(Plane.WorldXY, sw, sh);
            core_area = (sw * sh) * e;
            test_core(ref cores, ref grid_pts, ref grid_val);
        }

        public void test_core(ref List<Rectangle3d> core_list, ref List<Point3d> g_pts, ref DataTree<bool> g_val)
        {
            List<Rectangle3d> c = new List<Rectangle3d>();

            double core_area_min = core_area * (1.0 - deviation);
            double core_area_max = core_area * (1.0 + deviation);

            for (int i = 1; i <= skin_width; i++)
            {
                for(int j = 1; j <= skin_height; j++)
                {

                    g_pts.Add(new Point3d(i - 0.5, j - 0.5,0));

                    if ((i * j == core_area || (i * j >= core_area_min && i * j <= core_area_max)) && i >= core_min_width && j >= core_min_height)
                    {
                        double possible_x_pos = skin.Width - i;
                        double possible_y_pos = skin.Height - j;

                        for (int k = 0; k <= possible_x_pos; k++)
                        {
                            for (int l = 0; l <= possible_y_pos; l++)
                            {
                                c.Add(new Rectangle3d(Plane.WorldXY, new Point3d(k, l, 0), new Point3d(k + i, l + j, 0)));
                                g_val.EnsurePath(c.Count() - 1);
                                
                                for (int m = 0; m < skin_width; m++)
                                {
                                    for (int n = 0; n < skin_height; n++)
                                    {
                                        if ((m + 0.5 > k && m + 0.5 < k + i) && (n + 0.5 > l && n + 0.5 < l + j))
                                        {
                                            g_val.Add(false);
                                        }
                                        else
                                        {
                                            g_val.Add(true);
                                        }                                        
                                    }
                                }
                            }
                        }
                    }                        
                }
            }
            core_list = c;            
        }
    }
}
