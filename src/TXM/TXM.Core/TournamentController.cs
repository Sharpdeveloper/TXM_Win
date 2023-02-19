using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TXM.Core
{
    public class TournamentController
    {
        public Tournament ActiveTournament { get; private set; }
        private bool firststart = false;
        private ITimerWindow timerWindow;
        public TournamentTimer ActiveTimer { get; private set; }
        public IO ActiveIO { get; private set; }
        public bool Started { get; private set; }
        private IProjectorWindow projectorWindow;

        public TournamentController(IO _io)
        {
            ActiveTimer = new TournamentTimer();
            ActiveIO = _io;
            Started = false;
        }

        public void NewPlayer(IPlayerDialog ipd)
        {
            ipd.ShowDialog();
            if (ipd.GetDialogResult())
            {
                ActiveTournament.AddPlayer(ipd.GetPlayer());
            }
        }

        public void NewTournament(ITournamentDialog itd)
        {
            if (ActiveTournament != null)
            {
                if (!ActiveIO.ShowMessageWithOKCancel("The current tournament will be overwritten."))
                    return;
            }
            itd.SetIO(ActiveIO);
            itd.ShowDialog();
            if (itd.GetDialogResult())
            {
                ActiveTournament = itd.GetTournament();
                ActiveTimer.DefaultTime = ActiveTournament.Rule.DefaultTime;

            }
        }

        public bool StartTournament(string buttonGetResultsText, bool CutIsEnabled)
        {
            if (ActiveTournament.Participants.Count != 0)
            {
                firststart = true;
                Started = true;
                ActiveIO.Save(ActiveTournament, true, buttonGetResultsText, CutIsEnabled, "TournamentStart");
                return true;
            }
            else
            {
                ActiveIO.ShowMessage("Tournament can't be started without player");
                return false;
            }
        }

        public void EditPlayer(IPlayerDialog ipd, int index)
        {
            Player xwp = ActiveTournament.Participants[index];
            ipd.SetPlayer(xwp);
            ipd.SetTitle("Player " + xwp.Nickname + " change");
            ipd.SetButtonOKText("Accept changes.");
            ipd.ShowDialog();
            if (ipd.GetDialogResult() && ipd.IsChanged())
            {
                ChangePlayer(index, ipd.GetPlayer());
            }
        }

        private void ChangePlayer(int index, Player player)
        {
            Player p = ActiveTournament.Participants[index];
            p.Team = player.Team;
            p.Name = player.Name;
            p.Firstname = player.Firstname;
            p.HasWonBye = player.HasWonBye;
            p.HasListGiven = player.HasListGiven;
            p.HasPaid = player.HasPaid;
            p.TableNo = player.TableNo;
            p.IsPresent = player.IsPresent;
        }

        private void ChangeTournament(Tournament tournament)
        {
            ActiveTournament.Name = tournament.Name;
            ActiveTournament.Cut = tournament.Cut;
            ActiveTournament.MaxPoints = tournament.MaxPoints;
            ActiveTournament.TeamProtection = tournament.TeamProtection;
            ActiveTournament.PrintDDGER = tournament.PrintDDGER;
            ActiveTournament.PrintDDENG = tournament.PrintDDENG;
            ActiveTournament.Single = tournament.Single;
            ActiveTournament.Rule = tournament.Rule;
            ActiveTimer.DefaultTime = ActiveTournament.Rule.DefaultTime;
        }

        public void Import(ITournamentDialog itd, bool csv)
        {
            if (csv)
            {
                ActiveTournament = ActiveIO.CSVImport();
            }
            else
            {
                ActiveTournament = ActiveIO.GOEPPImport();
            }
            if (ActiveTournament != null)
            {
                itd.SetTournament(ActiveTournament);
                itd.SetIO(ActiveIO);
                itd.ShowDialog();
                if (itd.IsChanged())
                {
                    ChangeTournament(itd.GetTournament());
                }
                ActiveTimer.DefaultTime = ActiveTournament.Rule.DefaultTime;
            }
        }

        public void GOEPPExport()
        {
            ActiveIO.GOEPPExport(ActiveTournament);
        }

        public void Save(string GetResultsText, bool CutIsEnabled, bool autosave = false, string text = "Pairings_Round")
        {
            ActiveIO.Save(ActiveTournament, autosave, GetResultsText, CutIsEnabled, text + ActiveTournament.Rounds.Count);
        }

        public List<Pairing> GetSeed(bool cut)
        {
            List<Pairing> temp = ActiveTournament.GetSeed(firststart, cut);
            firststart = false;
            return temp;
        }

        public bool GetResults(List<Pairing> pairings, string buttonGetResultsText, bool CutIsEnabled, bool update = false, bool end = false)
        {
            if (update)
            {
                //ActiveTournament.GetResults(pairings, true);
                return true;
            }
            if (pairings.Count == 1)
                end = true;
            bool allResultsEdited = true;
            if (ActiveTournament.Rule.IsDrawPossible || ActiveTournament.bonus)
            {
                foreach (Pairing p in pairings)
                {
                    if (!p.IsResultEdited)
                    {
                        allResultsEdited = false;
                        break;
                    }
                }
            }
            if (allResultsEdited)
            {
                //if (CheckResults(pairings) || ActiveTournament.bonus)
                //{
                //    //ActiveTournament.GetResults(pairings);
                //}
                //else
                //{
                //    ActiveIO.ShowMessage("One ore more results are invalid.");
                //    return false;
                //}
            }
            else
            {
                ActiveIO.ShowMessage("There is a result missing.");
                return false;
            }

            if (end)
            {
                if (!ActiveTournament.CutStarted)
                {
                    ActiveTournament.CalculateWonBye();
                }
                //DataGridPairing.Visibility = System.Windows.Visibility.Hidden;
                //ChangeGUIState(false, true);
            }
            else
            {
                //ChangeGUIState(false);
            }
            ActiveTournament.Sort();
            return true;
        }

        public void ShowTimerWindow(ITimerWindow itw)
        {
            timerWindow = itw;
            timerWindow.SetTimer(ActiveTimer);
            string imgurl = ActiveIO.ActiveSettings.BGImagePath;
            if (imgurl != "")
            {
                timerWindow.SetImage(new Uri(imgurl));
            }
            timerWindow.SetLabelColor(ActiveIO.ActiveSettings.TextColor);
            timerWindow.SetTextSize(ActiveIO.ActiveSettings.TextSize);
            timerWindow.Show();
        }

        public void Load(IAutoSaveDialog iad, bool autosave)
        {
            bool overwrite = true;
            string filename = "";
            if (autosave)
            {
                string[] filenames = ActiveIO.GetAutosaveFiles();
                List<AutosaveFile> files = new List<AutosaveFile>();
                for (int i = filenames.Length - 1; i >= 0; i--)
                {
                    files.Add(new AutosaveFile(filenames[i]));
                }
                iad.Init(files);
                iad.ShowDialog();
                overwrite = iad.GetDialogReturn();
                filename = iad.GetFileName();
            }
            else
            {
                if (ActiveTournament != null)
                {
                    if (!ActiveIO.ShowMessageWithOKCancel("The current tournament will be overwritten."))
                        overwrite = false;
                }
            }
            if (overwrite == true)
            {
                ActiveTournament = ActiveIO.Load(filename);
            }
        }

        //private bool CheckResults(List<Pairing> pairings)
        //{
        //    //Todo in Rules auslagern
        //    foreach (Pairing p in pairings)
        //    {
        //        //if (p.Player1Score != 0 && p.Player1Score < 12)
        //        //    return false;
        //        //if (p.Player2Score != 0 && p.Player2Score < 12)
        //        //    return false;
        //        if (!ActiveTournament.Rule.IsDrawPossible)
        //        {
        //            if (p.Player1Score == p.Player2Score && p.Winner == "Automatic" && p.Player2 != ActiveTournament.Bye && p.Player2 != ActiveTournament.WonBye)
        //                return false;
        //        }
        //    }
        //    return true;
        //}

        public void EditTournament(ITournamentDialog itd)
        {
            itd.SetGameSystemIsChangeable(false);
            itd.SetTournament(ActiveTournament);
            itd.ShowDialog();
            if (itd.IsChanged())
            {
                ChangeTournament(itd.GetTournament());
            }
        }

        public void RemovePlayer(int index, bool disqualify = false)
        {
            Player player = ActiveTournament.Participants[index];
            string text = "remove";
            if (Started)
            {
                text = "drop";
            }
            else if (Started & disqualify)
            {
                text = "disqualify";
            }
            if (ActiveIO.ShowMessageWithOKCancel("Do you really want to " + text + " " + player.DisplayName + "?"))
            {
                if (text == "remove")
                {
                    ActiveTournament.RemovePlayer(player);
                }
                else if (text == "drop")
                {
                    ActiveTournament.DropPlayer(player);
                }
                else if (text == "disqualify")
                {
                    ActiveTournament.DisqualifyPlayer(player);
                }
            }
        }

        public void RemovePlayer(Player p, bool disqualify = false)
        {
            RemovePlayer(ActiveTournament.GetIndexOfPlayer(p));
        }

        public void EditPairings(IPairingDialog ipd, string buttonGetResultsText, bool CutIsEnabled)
        {

            ipd.SetParticipants(ActiveTournament.Participants);
            ipd.SetPairings(ActiveTournament.Pairings);
            ipd.ShowDialog();
            if (ipd.GetDialogResult())
            {
                ActiveTournament.Pairings = ipd.GetPairings();

                ActiveIO.Save(ActiveTournament, true, buttonGetResultsText, CutIsEnabled, "ChangePairings");
            }
        }

        public List<Pairing> ResetLastResults()
        {
            List<Pairing> pl = new List<Pairing>();
            foreach (var p in ActiveTournament.Rounds[ActiveTournament.Rounds.Count - 1].Pairings)
                pl.Add(new Pairing(p) { IsResultEdited = true, });
            ActiveTournament.RemoveLastRound();
            return pl;
        }

        public string Print(bool print, bool pairings = false, bool results = false)
        {
            string file = "";
            if (!pairings && ActiveTournament != null)
            {
                file = ActiveIO.PrintPlayerList(ActiveTournament);
            }
            else if (pairings)
            {
                file = ActiveIO.PrintPairings(ActiveTournament, results);
            }

            if (print)
            {
                var uri = "file://" + file;
                var psi = new ProcessStartInfo();
                psi.UseShellExecute = true;
                psi.FileName = uri;
                Process.Start(psi);
            }

            return file;
        }

        public void PrintScoreSheet()
        {
            string file = ActiveIO.PrintScoreSheet(ActiveTournament);
            var uri = "file://" + file;
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            Process.Start(psi);
        }

        public void StartTimer(string startTime)
        {
            bool timeOK = startTime == "";
            if (timeOK)
            {
                ActiveTimer.StartTimer();
            }
            else
            {
                int pos = startTime.IndexOf(':');
                int check = startTime.LastIndexOf(':');
                int starthour = 0, startmin = 0;
                timeOK = true;
                if (pos != check)
                {
                    timeOK = false;
                }
                if (pos >= 3 || pos == 0)
                {
                    timeOK = false;
                }
                if ((pos == 1 && startTime.Length >= 5) || startTime.Length >= 6)
                {
                    timeOK = false;
                }
                try
                {
                    starthour = Int32.Parse(startTime.Substring(0, pos));
                }
                catch (Exception)
                {
                    timeOK = false;
                }
                try
                {
                    startmin = Int32.Parse(startTime.Substring(pos + 1));
                }
                catch (Exception)
                {
                    timeOK = false;
                }
                if (timeOK)
                {
                    ActiveTimer.StartTimer(starthour, startmin);
                }
                else
                {
                    ActiveIO.ShowMessage("The Time is only allowed in the format HH:MM.");
                }
            }
        }

        public void PauseTimer()
        {
            ActiveTimer.PauseTimer();
        }

        public void ResetTimer()
        {
            ActiveTimer.ResetTimer();
        }

        public void ShowAbout(IInfoDialog iad)
        {
            iad.SetText("© " + TXM.Core.Settings.COPYRIGHTYEAR + " Sharpdeveloper aka TKundNobody\nTXM Version: " + TXM.Core.Settings.TXMVERSION + "\n© Icons: Icons8 (www.icons8.com)");
            iad.ShowDialog();
        }

        public void ShowThanks(IInfoDialog iad)
        {
            iad.SetText("Special Thanks to following Friends and Tester:\n\nBarlmoro - Tester, User and the Reason for at least half of the features.\ntgbrain - Teammate and tester\nKyle_Nemesis - Tester\nPhoton - User who finds every weird error\nN4-DO - Creater of the TXM-Logo\nMercya - Tester\nBackfire84 - Poweruser\nGreenViper - Tester with the ability to find stranger errors\nCarnis - Tester");
            iad.ShowDialog();
        }

        public void ShowSupport(IInfoDialog iad)
        {
            iad.SetText("If you like TXM and want to support it, you can do it ins serval ways:\n\n* Spread the word, the more use it, the better it gets!\n* Test everything!\n* Help to add more Games. More information click the Button 'New Game Manual'\n* Leave a tip. If you want to leave a tip, click the Button 'Donate a Tip'");
            iad.ShowDialog();
        }

        public void ShowProjector(IProjectorWindow ipw, bool table)
        {
            if (ActiveTournament != null && ActiveTournament.Rounds.Count > 0)
            {
                if (projectorWindow == null || projectorWindow.IsClosed())
                    projectorWindow = ipw;
                string file = "";
                string title = "";
                if (table)
                {
                    file = Print(false);
                    title = ActiveTournament.Name + " - Standing";
                }
                else
                {

                    file = Print(false, true);
                    title = ActiveTournament.Name + " - Pairings";
                }
                projectorWindow.SetURL(file);
                projectorWindow.SetTitle(title);
                projectorWindow.SetTimer(ActiveTimer);
                projectorWindow.Show();
            }
        }

        public void GetBBCode(IOutputDialog iod, IClipboard ic)
        {
            iod.ShowDialog();
            if (iod.GetDialogResult())
            {
                StringBuilder sb = new StringBuilder();
                if (iod.IsResultOutput())
                {
                    sb.Append(ActiveIO.CreateOutputForPairings(ActiveTournament, true, true));
                }
                if (iod.IsTableOutput())
                {
                    sb.Append(ActiveIO.CreateOutputForPairings(ActiveTournament, true, false));
                }
                if (iod.IsPairingOutput())
                {
                    sb.Append(ActiveIO.CreateOutputForTable(ActiveTournament, true));
                }
                ic.SetText(sb.ToString());
            }
        }

        public void GetJSON(IClipboard ic)
        {
            (string json, string file) result = ActiveIO.GetJsonForListfortress(ActiveTournament);
            ic.SetText(result.json);
            ActiveIO.ShowMessage($"The Tournament is in your clipboard you can paste it to listfortress.\nAlternative you finde the here: {result.file}");
        }

        public string SetTimer(string time)
        {
            try
            {
                ActiveTimer.DefaultTime = Int32.Parse(time);
            }
            catch (Exception)
            {
                try
                {
                    ActiveTimer.DefaultTime = ActiveTournament.Rule.DefaultTime;
                }
                catch (Exception)
                {
                    ActiveTimer.DefaultTime = 0;
                }
            }

            return ActiveTimer.DefaultTime.ToString();
        }

        public string SetRandomTime(string time)
        {
            try
            {
                ActiveTimer.RandomMins = Int32.Parse(time);
            }
            catch (Exception)
            {
                try
                {
                    ActiveTimer.RandomMins = ActiveTournament.Rule.DefaultRandomMins;
                }
                catch (Exception)
                {
                    ActiveTimer.RandomMins = 0;
                }
            }

            return ActiveTimer.RandomMins.ToString();
        }

        public string GetRandomTime()
        {
            if(ActiveTournament != null && ActiveTournament.Rule != null && ActiveTournament.Rule.DefaultRandomMins != 0)
            {
                ActiveTimer.RandomTime = true;
                if (ActiveTimer.RandomMins != 0)
                    return ActiveTimer.RandomMins.ToString();
                else
                    return SetRandomTime(ActiveTournament.Rule.DefaultRandomMins.ToString());
            }
            ActiveTimer.RandomTime = false;
            return "";
        }

        public void SetImage()
        {
            if (ActiveIO.NewImage())
            {
                if (timerWindow != null && !timerWindow.SetImage(new Uri(ActiveIO.TempImgPath)))
                {
                    ActiveIO.ShowMessage("The Image is invalid.");
                }
            }
        }

        public void SetTimerLabelColor(string color)
        {
            ActiveIO.ActiveSettings.TextColor = color;
            ActiveIO.SaveSettings();
            if (timerWindow != null)
            {
                timerWindow.SetLabelColor(color);
            }
        }

        public void SetTimerTextSize(double size)
        {
            ActiveIO.ActiveSettings.TextSize = size;
            ActiveIO.SaveSettings();
            if (timerWindow != null)
            {
                timerWindow.SetTextSize(size);
            }
        }

        public void Close()
        {
            if (timerWindow != null)
                timerWindow.Quit();
            if (projectorWindow != null)
                projectorWindow.Quit();
        }

        public void CalculateWonByes()
        {
            ActiveTournament.CalculateWonBye();
        }

        public void ShowUserManual()
        {
            var uri = "https://github.com/Sharpdeveloper/TXM/wiki/User-Manual";
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            Process.Start(psi);
        }

        public ObservableCollection<Pairing> AwardBonusPoints()
        {
            return ActiveTournament.GetBonusSeed();
        }

        public void AddCSV()
        {
            ActiveIO.CSVImportAdd(ActiveTournament);
        }
    }
}
