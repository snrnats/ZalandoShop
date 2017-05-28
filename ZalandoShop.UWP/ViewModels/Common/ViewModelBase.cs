using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight.Messaging;
using MetroLog;
using Microsoft.Toolkit.Uwp;
using Newtonsoft.Json;

namespace ZalandoShop.UWP.ViewModels.Common
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public const string NavigationParameterFolder = "NavigationParameter";
        protected readonly ILogger Logger;

        public ViewModelBase()
        {
            Logger = LogManagerFactory.DefaultLogManager.GetLogger(GetType().FullName);
        }

        public ViewModelBase(IMessenger messenger) : base(messenger)
        {
            Logger = LogManagerFactory.DefaultLogManager.GetLogger(GetType().FullName);
        }

        /// <summary>
        /// Override this method to handle activation of the page associated with this <see cref="ViewModelBase"/>
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Activate(object parameter)
        {
        }

        /// <summary>
        /// Override this method to handle deactivation of the page associated with this <see cref="ViewModelBase"/>
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Deactivate(object parameter)
        {
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns>
        ///     Id that can be used to retrieve the parameter using <see cref="GetNavigationParameter{T}" />. Returns
        ///     <see langword="null" /> if case of failure.
        /// </returns>
        protected async Task<string> SetNavigationParameter<T>(T parameter)
        {
            string filename = null;
            try
            {
                var folder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(NavigationParameterFolder, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
                var json = JsonConvert.SerializeObject(parameter);
                filename = parameter.GetType().FullName;
                await folder.WriteTextToFileAsync(json, filename).ConfigureAwait(false);
            }
            catch (IOException e)
            {
                Logger.Error("Failed to set navigation parameter", e);
            }
            catch (JsonException e)
            {
                Logger.Error("Failed to set navigation parameter", e);
            }

            return filename;
        }


        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     Paremeter that was set using <see cref="SetNavigationParameter{T}" />. Returns <see langword="null" /> in case
        ///     of failure.
        /// </returns>
        protected async Task<T> GetNavigationParameter<T>(string parameterId)
        {
            var result = default(T);
            try
            {
                var folder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(NavigationParameterFolder, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
                var filename = parameterId;
                var json = await folder.ReadTextFromFileAsync(filename).ConfigureAwait(false);
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (IOException e)
            {
                Logger.Error("Failed to get navigation parameter", e);
            }
            catch (JsonException e)
            {
                Logger.Error("Failed to get navigation parameter", e);
            }
            return result;
        }
    }
}
