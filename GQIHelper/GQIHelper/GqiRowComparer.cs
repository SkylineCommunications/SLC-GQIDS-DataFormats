// namespace Common.RealTimeUpdates
// {
//    using System;
//    using System.Collections.Generic;

//    using Skyline.DataMiner.Analytics.GenericInterface;

    // public class GqiRowComparer : IEqualityComparer<GQIRow>
    // {
    //     public static readonly GqiRowComparer Instance = new GqiRowComparer();

    //     public bool Equals(GQIRow x, GQIRow y)
    //     {
    //         if (ReferenceEquals(x, y))
    //         {
    //             return true;
    //         }

    //         if (x == null || y == null ||
    //             !String.Equals(x.Key, y.Key) ||
    //             x.Cells.Length != y.Cells.Length)
    //         {
    //             return false;
    //         }

    //         for (int i = 0; i < x.Cells.Length; i++)
    //         {
    //             var cx = x.Cells[i];
    //             var cy = y.Cells[i];

    //             if (!EqualityComparer<object>.Default.Equals(cx.Value, cy.Value) ||
    //                 !String.Equals(cx.DisplayValue, cy.DisplayValue))
    //             {
    //                 return false;
    //             }
    //         }

    //         return true;
    //     }

    //     public int GetHashCode(GQIRow row)
    //     {
    //         return row.Key?.GetHashCode() ?? 0;
    //     }
    // }
// }
