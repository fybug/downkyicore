﻿using DownKyi.Core.BiliApi.VideoStream.Models;
using DownKyi.Images;
using DownKyi.Models;
using DownKyi.Services;
using DownKyi.Utils;
using Prism.Commands;
using Prism.Services.Dialogs;
using IDialogService = DownKyi.PrismExtension.Dialog.IDialogService;

namespace DownKyi.ViewModels.DownloadManager
{
    public class DownloadingItem : DownloadBaseItem
    {
        public DownloadingItem() : this(null)
        {
        }

        public DownloadingItem(IDialogService? dialogService) : base(dialogService)
        {
            // 暂停继续按钮
            StartOrPause = ButtonIcon.Instance().Pause;
            StartOrPause.Fill = DictionaryResource.GetColor("ColorPrimary");

            // 删除按钮
            Delete = ButtonIcon.Instance().Delete;
            Delete.Fill = DictionaryResource.GetColor("ColorPrimary");
        }

        // model数据
        private Downloading downloading;
        public Downloading Downloading
        {
            get => downloading;
            set
            {
                downloading = value;

                switch (value.DownloadStatus)
                {
                    case DownloadStatus.NotStarted:
                    case DownloadStatus.WaitForDownload:
                        StartOrPause = ButtonIcon.Instance().Pause;
                        break;
                    case DownloadStatus.PauseStarted:
                        StartOrPause = ButtonIcon.Instance().Start;
                        break;
                    case DownloadStatus.Pause:
                        StartOrPause = ButtonIcon.Instance().Start;
                        break;
                    case DownloadStatus.Downloading:
                        StartOrPause = ButtonIcon.Instance().Pause;
                        break;
                    case DownloadStatus.DownloadSucceed:
                        // 下载成功后会从下载列表中删除
                        // 不会出现此分支
                        break;
                    case DownloadStatus.DownloadFailed:
                        StartOrPause = ButtonIcon.Instance().Retry;
                        break;
                    default:
                        break;
                }
                StartOrPause.Fill = DictionaryResource.GetColor("ColorPrimary");
            }
        }

        // 视频流链接
        public PlayUrl PlayUrl { get; set; }

        // 正在下载内容（音频、视频、弹幕、字幕、封面）
        public string DownloadContent
        {
            get => Downloading.DownloadContent;
            set
            {
                Downloading.DownloadContent = value;
                RaisePropertyChanged("DownloadContent");
            }
        }

        // 下载状态显示
        public string DownloadStatusTitle
        {
            get => Downloading.DownloadStatusTitle;
            set
            {
                Downloading.DownloadStatusTitle = value;
                RaisePropertyChanged("DownloadStatusTitle");
            }
        }

        // 下载进度
        public float Progress
        {
            get => Downloading.Progress;
            set
            {
                Downloading.Progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        //  已下载大小/文件大小
        public string DownloadingFileSize
        {
            get => Downloading.DownloadingFileSize;
            set
            {
                Downloading.DownloadingFileSize = value;
                RaisePropertyChanged("DownloadingFileSize");
            }
        }

        //  下载速度
        public string SpeedDisplay
        {
            get => Downloading.SpeedDisplay;
            set
            {
                Downloading.SpeedDisplay = value;
                RaisePropertyChanged("SpeedDisplay");
            }
        }

        // 操作提示
        private string operationTip;
        public string OperationTip
        {
            get => operationTip;
            set => SetProperty(ref operationTip, value);
        }

        #region 控制按钮

        private VectorImage startOrPause;
        public VectorImage StartOrPause
        {
            get => startOrPause;
            set
            {
                SetProperty(ref startOrPause, value);

                OperationTip = value.Equals(ButtonIcon.Instance().Start) ? DictionaryResource.GetString("StartDownload")
                    : value.Equals(ButtonIcon.Instance().Pause) ? DictionaryResource.GetString("PauseDownload")
                    : value.Equals(ButtonIcon.Instance().Retry) ? DictionaryResource.GetString("RetryDownload") : null;
            }
        }

        private VectorImage delete;
        public VectorImage Delete
        {
            get => delete;
            set => SetProperty(ref delete, value);
        }

        #endregion

        #region 命令申明

        // 下载列表暂停继续事件
        private DelegateCommand startOrPauseCommand;
        public DelegateCommand StartOrPauseCommand => startOrPauseCommand ?? (startOrPauseCommand = new DelegateCommand(ExecuteStartOrPauseCommand));

        /// <summary>
        /// 下载列表暂停继续事件
        /// </summary>
        private void ExecuteStartOrPauseCommand()
        {
            switch (Downloading.DownloadStatus)
            {
                case DownloadStatus.NotStarted:
                case DownloadStatus.WaitForDownload:
                    Downloading.DownloadStatus = DownloadStatus.PauseStarted;
                    DownloadStatusTitle = DictionaryResource.GetString("Pausing");
                    StartOrPause = ButtonIcon.Instance().Start;
                    StartOrPause.Fill = DictionaryResource.GetColor("ColorPrimary");
                    break;
                case DownloadStatus.PauseStarted:
                    Downloading.DownloadStatus = DownloadStatus.WaitForDownload;
                    DownloadStatusTitle = DictionaryResource.GetString("Waiting");
                    StartOrPause = ButtonIcon.Instance().Pause;
                    StartOrPause.Fill = DictionaryResource.GetColor("ColorPrimary");
                    break;
                case DownloadStatus.Pause:
                    Downloading.DownloadStatus = DownloadStatus.WaitForDownload;
                    DownloadStatusTitle = DictionaryResource.GetString("Waiting");
                    StartOrPause = ButtonIcon.Instance().Pause;
                    StartOrPause.Fill = DictionaryResource.GetColor("ColorPrimary");
                    break;
                //case DownloadStatus.PAUSE_TO_WAIT:
                case DownloadStatus.Downloading:
                    Downloading.DownloadStatus = DownloadStatus.Pause;
                    DownloadStatusTitle = DictionaryResource.GetString("Pausing");
                    StartOrPause = ButtonIcon.Instance().Start;
                    StartOrPause.Fill = DictionaryResource.GetColor("ColorPrimary");
                    break;
                case DownloadStatus.DownloadSucceed:
                    // 下载成功后会从下载列表中删除
                    // 不会出现此分支
                    break;
                case DownloadStatus.DownloadFailed:
                    Downloading.DownloadStatus = DownloadStatus.WaitForDownload;
                    DownloadStatusTitle = DictionaryResource.GetString("Waiting");
                    StartOrPause = ButtonIcon.Instance().Pause;
                    StartOrPause.Fill = DictionaryResource.GetColor("ColorPrimary");
                    break;
                default:
                    break;
            }
        }

        // 下载列表删除事件
        private DelegateCommand? deleteCommand;
        public DelegateCommand DeleteCommand => deleteCommand ??= new DelegateCommand(ExecuteDeleteCommand);

        /// <summary>
        /// 下载列表删除事件
        /// </summary>
        private async void ExecuteDeleteCommand()
        {
            var alertService = new AlertService(DialogService);
            var result = await alertService.ShowWarning(DictionaryResource.GetString("ConfirmDelete"), 2);
            if (result != ButtonResult.OK)
            {
                return;
            }
            
            App.DownloadingList?.Remove(this);
        }

        #endregion

    }
}
