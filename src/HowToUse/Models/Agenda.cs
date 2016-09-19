using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HowToUse.Models {
    public class Agenda {
        public Agenda() {
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
        }

        public int Id { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public byte[] Schedule { get; internal set; }

        public IEnumerable<TimeSpan> Items {
            get {
                if (Schedule == null || Schedule.Length == 0) {
                    return Enumerable.Empty<TimeSpan>();
                }

                using (var memory = new MemoryStream(Schedule)) {
                    using (var reader = new BinaryReader(memory)) {
                        return Read(reader);
                    }
                }
            }
            set {
                using (var memory = new MemoryStream()) {
                    using (var writer = new BinaryWriter(memory)) {
                        Write(writer, value);
                    }

                    Schedule = memory.ToArray();
                }
            }
        }

        internal virtual IEnumerable<TimeSpan> Read(BinaryReader reader) {
            if (reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }

            var count = reader.ReadInt32();
            var items = new TimeSpan[count];
            for (int index = 0; index != count; ++index) {
                var ticks = reader.ReadInt64();

                items[index] = TimeSpan.FromTicks(ticks);
            }

            return items;
        }

        internal virtual void Write(BinaryWriter writer, IEnumerable<TimeSpan> items) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            writer.Write(items.Count());
            foreach (var item in items) {
                writer.Write(item.Ticks);
            }
        }
    }
}
