using System;
using System.Collections;
using System.Collections.Generic;

namespace _1002
{

    public class StationSearcher : IEnumerable, IEnumerator
    {
        //XML문서에서 받아온 충전소 정보
        public List<ChargeStation> stationlist = new List<ChargeStation>();

        #region 싱글턴
        public static StationSearcher Instance { get; private set; }
        static StationSearcher()
        {
            Instance = new StationSearcher();
        }
        private StationSearcher()
        { }
        #endregion

        #region 인덱서쓸려구
        private int position = -1;
        public object Current { get { return stationlist[position]; } }
        #endregion

        #region 인덱서
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < stationlist.Count; i++)
            {
                yield return (stationlist[i]);
            }
        }

        public bool MoveNext()
        {
            if (position == stationlist.Count - 1)
            {
                Reset();
                return false;
            }
            position++;
            return (position < stationlist.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        public ChargeStation this[int index]
        {
            get
            {
                if (index < 0 || index >= stationlist.Count)
                {
                    throw new Exception("잘못된 인덱스");
                }
                else
                {
                    return stationlist[index];
                }
            }
            private set { }
        }

        #endregion

        internal void Clear()
        {
            stationlist.Clear();
        }

        internal void Add(ChargeStation ch)
        {
            stationlist.Add(ch);
        }


    }

}
