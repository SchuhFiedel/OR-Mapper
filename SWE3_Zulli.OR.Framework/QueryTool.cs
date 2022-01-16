using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWE3_Zulli.OR.Framework.MetaModel;

namespace SWE3_Zulli.OR.Framework
{
    /*
    public sealed class QueryTool<T> : IEnumerable<T>
    {

        private QueryTool<T> _Previous;

        private QueryOp _Op = QueryOp.NOP;

        private object[] _Args = null;

        private List<T> _InternalValues = null;



        internal QueryTool(QueryTool<T> previous)
        {
            _Previous = previous;
        }


        private void _Fill(Type t, ICollection<object> localCache)
        {
            List<QueryTool<T>> ops = new List<QueryTool<T>>();

            QueryTool<T> q = this;
            while (q != null)
            {
                ops.Insert(0, q);
                q = q._Previous;
            }

            __Entity ent = t._GetEntity();

            string sql = ent.GetSQL();
            List<Tuple<string, object>> parameters = new List<Tuple<string, object>>();
            string conj = (string.IsNullOrWhiteSpace(ent.SubsetQuery) ? " WHERE (" : " AND (");
            bool not = false;
            string opbrk = "";
            string clbrk = "";
            int n = 0;
            string op;

            __Field field;
            foreach (QueryTool<T> i in ops)
            {
                switch (i._Op)
                {
                    case QueryOp.OR:
                        if (!conj.EndsWith("(")) { conj = " OR "; }
                        break;
                    case QueryOp.NOT:
                        not = true; break;
                    case QueryOp.GRP:
                        opbrk += "("; break;
                    case QueryOp.ENDGRP:
                        clbrk += ")"; break;
                    case QueryOp.EQUALS:
                    case QueryOp.LIKE:
                        field = ent.GetFieldByName((string)i._Args[0]);

                        if (i._Op == QueryOp.LIKE)
                        {
                            op = (not ? " NOT LIKE " : " LIKE ");
                        }
                        else { op = (not ? " != " : " = "); }

                        sql += clbrk + conj + opbrk;
                        sql += (((bool)i._Args[2] ? "Lower(" + field.ColumnName + ")" : field.ColumnName) + op +
                                ((bool)i._Args[2] ? "Lower(:p" + n.ToString() + ")" : ":p" + n.ToString()));

                        if ((bool)i._Args[2]) { i._Args[1] = ((string)i._Args[1]).ToLower(); }
                        parameters.Add(new Tuple<string, object>(":p" + n++.ToString(), field.ToColumnType(i._Args[1])));

                        opbrk = clbrk = ""; conj = " AND "; not = false;
                        break;
                    case QueryOp.IN:
                        field = ent.GetFieldByName((string)i._Args[0]);

                        sql += clbrk + conj + opbrk;
                        sql += field.ColumnName + (not ? " NOT IN (" : " IN (");
                        for (int k = 1; k < i._Args.Length; k++)
                        {
                            if (k > 1) { sql += ", "; }
                            sql += (":p" + n.ToString());
                            parameters.Add(new Tuple<string, object>(":p" + n++.ToString(), field.ToColumnType(i._Args[k])));
                        }
                        sql += ")";

                        opbrk = clbrk = ""; conj = " AND "; not = false;
                        break;
                    case QueryOp.GT:
                    case QueryOp.LT:
                        field = ent.GetFieldByName((string)i._Args[0]);

                        if (i._Op == QueryOp.GT)
                        {
                            op = (not ? " <= " : " > ");
                        }
                        else { op = (not ? " >= " : " < "); }

                        sql += clbrk + conj + opbrk;
                        sql += (field.ColumnName + op + ":p" + n.ToString());

                        parameters.Add(new Tuple<string, object>(":p" + n++.ToString(), field.ToColumnType(i._Args[1])));

                        opbrk = clbrk = ""; conj = " AND "; not = false;
                        break;
                }
            }

            if (!conj.EndsWith("(")) { sql += ")"; }
            ORMapper._FillList(t, _InternalValues, sql, parameters);
        }


        /// <summary>Gets the query values.</summary>
        private List<T> _Values
        {
            get
            {
                if (_InternalValues == null)
                {
                    _InternalValues = new List<T>();

                    if (typeof(T).IsAbstract || typeof(T)._GetTable().IsMaterial)
                    {
                        ICollection<object> localCache = null;
                        foreach (Type i in typeof(T)._GetChildTypes())
                        {
                            _Fill(i, localCache);
                        }
                    }
                    else { _Fill(typeof(T), null); }
                }

                return _InternalValues;
            }
        }



        private QueryTool<T> _SetOp(QueryOp op, params object[] args)
        {
            _Op = op;
            _Args = args;

            return new QueryTool<T>(this);
        }

        public QueryTool<T> Not()
        {
            return _SetOp(QueryOp.NOT);
        }


        public QueryTool<T> And()
        {
            return _SetOp(QueryOp.AND);
        }

        public QueryTool<T> Or()
        {
            return _SetOp(QueryOp.OR);
        }

        public QueryTool<T> BeginGroup()
        {
            return _SetOp(QueryOp.GRP);
        }


        public QueryTool<T> EndGroup()
        {
            return _SetOp(QueryOp.ENDGRP);
        }


        public QueryTool<T> Equals(string field, object value, bool ignoreCase = false)
        {
            return _SetOp(QueryOp.EQUALS, field, value, ignoreCase);
        }


        public QueryTool<T> Like(string field, object value, bool ignoreCase = false)
        {
            return _SetOp(QueryOp.LIKE, field, value, ignoreCase);
        }


        public QueryTool<T> In(string field, params object[] values)
        {
            List<object> v = new List<object>(values);
            v.Insert(0, field);
            return _SetOp(QueryOp.LIKE, v.ToArray());
        }


        public QueryTool<T> GreaterThan(string field, object value)
        {
            return _SetOp(QueryOp.GT, field, value);
        }

        public QueryTool<T> LessThan(string field, object value)
        {
            return _SetOp(QueryOp.LT, field, value);
        }

        public List<T> ToList()
        {
            return new List<T>(_Values);
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _Values.GetEnumerator();
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Values.GetEnumerator();
        }
    }
    */
}
