using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.App_Start.DWF
{
    public class DwfFieldEnumerator : IEnumerator<FormFieldModel>
    {
        private int _i;
        private List<FormFieldModel> _fields;

        public DwfFieldEnumerator(List<FormFieldModel> fields)
        {
            this._fields = fields;
            if (fields == null) this._fields = new List<FormFieldModel>();
            _i = -1;
        }

        public FormFieldModel Current
        {
            get
            {
                if (this._i < 0 || this._i >= this._fields.Count) return null;
                return this._fields[this._i];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            if (this._i >= this._fields.Count) return false;

            _i++;

            return this._i >= this._fields.Count;
        }

        public void Reset()
        {
            _i = -1;
        }
    }
}