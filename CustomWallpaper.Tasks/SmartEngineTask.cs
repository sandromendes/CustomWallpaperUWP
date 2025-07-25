using CustomWallpaper.Tasks.Logs;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace CustomWallpaper.Tasks
{
    public sealed class SmartEngineTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnCanceled;

            await BackgroundTaskLoggerHelper.InfoAsync(nameof(SmartEngineTask), "Task STARTED");

            try
            {
                // O SmartEngineLocator deve ter os serviços necessários
                //var smartEngineService = SmartEngineLocator.GetSmartEngineService();

                // Exemplo de lógica a ser implementada futuramente:
                // - Checar conexão com servidor de modelos
                // - Verificar se há atualizações de modelos
                // - Baixar e armazenar localmente os modelos mais recentes
                // - Atualizar referências locais
                // Tudo isso ainda precisa ser descoberto e definido.

                // await smartEngineService.CheckForModelUpdatesAsync();
                // await smartEngineService.DownloadLatestModelsAsync();
                // await smartEngineService.UpdateLocalModelCacheAsync();

                await Task.Delay(3000); // Simulação de tempo de tarefa

                await BackgroundTaskLoggerHelper.InfoAsync(nameof(SmartEngineTask), "Task FINISHED: SmartEngine models processed");
            }
            catch (Exception ex)
            {
                await BackgroundTaskLoggerHelper.ErrorAsync(nameof(SmartEngineTask), ex);
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await BackgroundTaskLoggerHelper.InfoAsync(nameof(SmartEngineTask), $"Task CANCELED. Reason: {reason}");
        }
    }
}
