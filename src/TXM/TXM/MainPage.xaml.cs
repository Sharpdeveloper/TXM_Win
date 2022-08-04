namespace TXM;

using TXM.Core;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

		labelVersion.Text = Settings.TXMVERSION;
		foreach (string rules in AbstractRules.GetAllRuleNames())
			labelSystems.Text = labelSystems.Text + rules + "\n";
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}


