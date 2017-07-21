using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            }

            if(ActiveTournament != null)
                ActiveTournament.Io = ActiveIO;
        }

        public bool StartTournament(bool GetResultsIsEnabled, bool NextRoundIsEnabled, bool CutIsEnabled)
        {
            if (ActiveTournament.Participants.Count != 0)
            {
                firststart = true;
                Started = true;
                ActiveIO.Save(ActiveTournament, true, GetResultsIsEnabled, NextRoundIsEnabled, CutIsEnabled, "TournamentStart");
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
            p.Forename = player.Forename;
            p.WonBye = player.WonBye;
            p.ListGiven = player.ListGiven;
            p.Paid = player.Paid;
            p.TableNo = player.TableNo;
            p.Present = player.Present;
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
            ActiveTournament.Io = ActiveIO;
        }

        public void Import(ITournamentDialog itd, bool csv)
        {
            if(csv)
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
            }
        }

        public void GOEPPExport()
        {
            ActiveIO.GOEPPExport(ActiveTournament);
        }

        public void Save(bool GetResultsIsEnabled, bool NextRoundIsEnabled, bool CutIsEnabled, bool autosave = false)
        {
            ActiveIO.Save(ActiveTournament, autosave, GetResultsIsEnabled, NextRoundIsEnabled, CutIsEnabled, "Pairings_Round" + ActiveTournament.Rounds.Count);
        }

        public List<Pairing> GetSeed(bool cut)
        {
            List<Pairing> temp = ActiveTournament.GetSeed(firststart, cut);
            firststart = false;
            return temp;
        }

        public bool GetResults(List<Pairing> pairings, bool GetResultsIsEnabled, bool NextRoundIsEnabled, bool CutIsEnabled, bool update = false, bool end = false)
        {
            if (update)
            {
                ActiveTournament.GetResults(pairings, true);
                return true;
            }
            if (pairings.Count == 1)
                end = true;
            bool allResultsEdited = true;
            if (ActiveTournament.Rule.IsDrawPossible)
            {
                foreach (Pairing p in pairings)
                {
                    if (!p.ResultEdited)
                    {
                        allResultsEdited = false;
                        break;
                    }
                }
            }
            if (allResultsEdited)
            {
                if (CheckResults(pairings))
                {
                    ActiveTournament.GetResults(pairings);
                }
                else
                {
                    ActiveIO.ShowMessage("One ore more results are invalid.");
                    return false;
                }
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
            
            ActiveIO.Save(ActiveTournament, true, GetResultsIsEnabled, NextRoundIsEnabled, CutIsEnabled, "Result_Round" + ActiveTournament.Rounds.Count);
            return true;
        }

        public void OpenTimerWindow(ITimerWindow itw)
        {
            timerWindow = itw;
            timerWindow.SetIO(ActiveIO);
            timerWindow.SetTimer(ActiveTimer);
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
                for(int i = 0; i < filenames.Length; i++)
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
                if(ActiveTournament != null)
                {
                    ActiveTournament.Io = ActiveIO;
                }
            }
        }

        private bool CheckResults(List<Pairing> pairings)
        {
            //Todo in Rules auslagern
            foreach (Pairing p in pairings)
            {
                //if (p.Player1Score != 0 && p.Player1Score < 12)
                //    return false;
                //if (p.Player2Score != 0 && p.Player2Score < 12)
                //    return false;
                if (!ActiveTournament.Rule.IsDrawPossible)
                {
                    if (p.Player1Score == p.Player2Score && p.Winner == "Automatic" && !p.Player1.Bye && !p.Player2.Bye)
                        return false;
                }
            }
            return true;
        }

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
            else if(Started & disqualify)
            {
                text = "disqualify";
            }
            if (ActiveIO.ShowMessageWithOKCancel("Do you really want to " + text + " " + player.DisplayName + "?"))
            {
                if (text == "remove")
                {
                    ActiveTournament.RemovePlayer(player);
                }
                else if(text == "drop")
                {
                    ActiveTournament.DropPlayer(player);
                }
                else if(text == "disqualify")
                {
                    ActiveTournament.DisqualifyPlayer(player);
                }
            }
        }

        public void EditPairings(IPairingDialog ipd, bool GetResultsIsEnabled, bool NextRoundIsEnabled, bool CutIsEnabled)
        {
            
            ipd.SetParticipants(ActiveTournament.Participants);
            ipd.SetPairings(ActiveTournament.Pairings);
            ipd.ShowDialog();
            if (ipd.GetDialogResult())
            {
                ActiveTournament.Pairings = ipd.GetPairings();
                
                ActiveIO.Save(ActiveTournament, true, GetResultsIsEnabled, NextRoundIsEnabled, CutIsEnabled, "ChangePairings");
            }
        }

        public List<Pairing> ResetLastResults()
        {
            List<Pairing> pl = new List<Pairing>();
            foreach (var p in ActiveTournament.Rounds[ActiveTournament.Rounds.Count - 1].Pairings)
                pl.Add(new Pairing(p) { ResultEdited = true, });
            ActiveTournament.RemoveLastRound();
            return pl;
        }

        public string Print(bool print, bool pairings = false, bool results = false)
        {
            string file = "";
            if(!pairings && ActiveTournament != null)
            {
                file = ActiveIO.Print(ActiveTournament);
            }
            else if(pairings)
            {
                file = ActiveIO.Print(ActiveTournament, results);
            }

            if(print)
                Process.Start("file://" + file);

            return file;
        }

        public void PrintScoreSheet()
        {
            string file = ActiveIO.PrintScoreSheet(ActiveTournament);

            Process.Start("file://" + file);
        }

        public void StartTimer()
        {
            ActiveTimer.StartTimer();
        }

        public void PauseTimer()
        {
            ActiveTimer.PauseTimer();
        }

        public void ResetTimer()
        {
            ActiveTimer.ResetTimer();
        }

        public void ShowAbout(IAboutDialog iad)
        {
            iad.SetText("© 2014 - 2017 Sharpdeveloper aka TKundNobody\nTXM Version: " + TXM.Core.Settings.TXMVERSION + "\nSpecial Thanks to following Friends and Tester:\nBarlmoro - Tester, User and the Reason for at least half of the features.\ntgbrain - Teammate and tester\nKyle_Nemesis - Tester\nPhoton - User who finds every weird error\nN4-DO - Creater of the TXM-Logo\nMercya - Tester\n© Icons: Icons8 (www.icons8.com)");
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
                projectorWindow.Show();
            }
        }

        public void GetBBCode(IOutputDialog iod, IClipboard ic)
        {
            iod.ShowDialog();
            if(iod.GetDialogResult())
            {
                StringBuilder sb = new StringBuilder();
                if(iod.IsResultOutput())
                {
                    sb.Append(ActiveIO.GetBBCode(ActiveTournament, false, true));
                }
                if(iod.IsTableOutput())
                {
                    sb.Append(ActiveIO.GetBBCode(ActiveTournament, true));
                }
                if(iod.IsPairingOutput())
                {
                    sb.Append(ActiveIO.GetBBCode(ActiveTournament, false));
                }
                ic.SetText(sb.ToString());
            }
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
                catch(Exception)
                {
                    ActiveTimer.DefaultTime = 0;
                }
            }

            return ActiveTimer.DefaultTime.ToString();
        }
    }
}
