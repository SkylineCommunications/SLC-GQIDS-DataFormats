namespace Common.RealTimeUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Net.Helper;

    public class GqiTableComparer
    {
        public GqiTableComparer(ICollection<GQIRow> oldRows, ICollection<GQIRow> newRows)
        {
            if (oldRows == null)
            {
                throw new ArgumentNullException(nameof(oldRows));
            }

            if (newRows == null)
            {
                throw new ArgumentNullException(nameof(newRows));
            }

            AddedRows = newRows.ExceptBy(oldRows, x => x.Key).ToList();

            RemovedRows = oldRows.ExceptBy(newRows, x => x.Key).ToList();

            UpdatedRows = oldRows.Join(
                newRows,
                oldRow => oldRow.Key,
                newRow => newRow.Key,
                (oldRow, newRow) => new { OldRow = oldRow, NewRow = newRow })
                .Where(x => !GqiRowComparer.Instance.Equals(x.OldRow, x.NewRow))
                .Select(x => x.NewRow)
                .ToList();
        }

        public ICollection<GQIRow> AddedRows { get; }

        public ICollection<GQIRow> UpdatedRows { get; }

        public ICollection<GQIRow> RemovedRows { get; }

        public override string ToString()
        {
            return $"{AddedRows.Count} added, {UpdatedRows.Count} updated, {RemovedRows.Count} removed";
        }
    }
}
