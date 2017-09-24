using PX.Data;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace BatchPaymentSFTPProvider
{
    public class PXMultipleProviderTypeSelectorAttribute : PXCustomSelectorAttribute
    {
        [Serializable]
        [PXHidden]
        public partial class ProviderRec : IBqlTable
        {
            #region TypeName
            public abstract class typeName : PX.Data.IBqlField { }
            [PXString(255, InputMask = "", IsKey = true)]
            [PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual string TypeName { get; set; }
            #endregion

            #region ProviderName
            public abstract class providerName : PX.Data.IBqlField { }
            [PXString(255, InputMask = "", IsKey = true)]
            [PXUIField(DisplayName = "Provider Name", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual string ProviderName { get; set; }
            #endregion
        }

        protected Type _MainProviderType;
        protected Type[] _DependentProviderType;

        public PXMultipleProviderTypeSelectorAttribute(Type mainProviderType, params Type[] dependentProviderType)
            : base(typeof(ProviderRec.typeName),
                   typeof(ProviderRec.providerName))
        {
            _MainProviderType = mainProviderType;
            _DependentProviderType = dependentProviderType;
        }

        protected virtual IEnumerable GetRecords()
        {
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (PXSubstManager.IsSuitableTypeExportAssembly(ass, false))
                {
                    Type[] types = null;
                    try
                    {
                        if (!ass.IsDynamic)
                            types = ass.GetExportedTypes();
                    }
                    catch (ReflectionTypeLoadException te)
                    {
                        types = te.Types;
                    }
                    catch
                    {
                        continue;
                    }
                    if (types != null)
                    {
                        foreach (Type t in types)
                        {
                            if (t != null && _MainProviderType.IsAssignableFrom(t) && t != _MainProviderType)
                            {
                                var dependents = _DependentProviderType.Where(dt => dt.IsAssignableFrom(t)).ToList();
                                if(dependents.Any())
                                {
                                    var nonBaseDependents = dependents.Where(dt => dt != t).ToList();
                                    if(nonBaseDependents.Any())
                                    {
                                        yield return new ProviderRec { TypeName = t.FullName, ProviderName = nonBaseDependents.Select(nbd => nbd.Name).LastOrDefault() };
                                    }
                                }
                                else
                                {
                                    yield return new ProviderRec { TypeName = t.FullName, ProviderName = _MainProviderType.Name };
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Static Methods
        public static bool IsProvider<TField, TProvider>(PXCache sender, object data)
            where TField : IBqlField
            where TProvider : class
        {
            var sel = (ProviderRec)Select<TField>(sender, data);
            return sel.ProviderName == typeof(TProvider).Name;
        }
        #endregion

    }

}
