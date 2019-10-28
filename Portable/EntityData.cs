using EntityComponentSystemCSharp.Components;
using EntityComponentSystemCSharp;
using static EntityComponentSystemCSharp.EntityManager;
using Inv;

namespace Portable
{
	/// <summary>
	/// Provides a table of standard display data for entities.
	/// </summary>
	internal class EntityData
	{
		Label _name;
		Label _health;
		Label _maxHealth;
		Label _attack;
		Label _accuracy;
		Label _defense;
		Label _gold;
		Label _speed;
		Table _table;


		public Table Table => _table;

		public EntityData(Surface surface, Colour fontColor, Colour backgroundColor)
		{
			_table = surface.NewTable();
			_table.Background.Colour = backgroundColor;
			_name = surface.NewLabel();
			var n = surface.NewLabel();
			n.Text = "Name:  ";

			_health = surface.NewLabel();
			_maxHealth = surface.NewLabel();
			var h = surface.NewLabel();
			h.Text = "Health:  ";
			var divider = surface.NewLabel();
			divider.Text = " / ";
			var healthPanel = surface.NewHorizontalStack();
			healthPanel.AddPanel(h);
			healthPanel.AddPanel(_health);
			var maxHealthPanel = surface.NewHorizontalStack();
			maxHealthPanel.AddPanel(divider);
			maxHealthPanel.AddPanel(_maxHealth);

			_attack = surface.NewLabel();
			var at = surface.NewLabel();
			at.Text = "Attack:  ";

			_accuracy = surface.NewLabel();
			var ac = surface.NewLabel();
			ac.Text = "Accuracy:  ";

			_defense = surface.NewLabel();
			var def = surface.NewLabel();
			def.Text = "Defense:  ";

			_speed = surface.NewLabel();
			var s = surface.NewLabel();
			s.Text = "Speed:  ";

			_gold = surface.NewLabel();
			var gl = surface.NewLabel();
			gl.Text = "Gold:  ";

			_table = surface.NewTable();
			_table.Compose(
				new Panel[,]
				{
					{n, _name},
					{healthPanel, maxHealthPanel},
					{at, _attack},
					{ac, _accuracy},
					{def, _defense},
					{s, _speed},
					{gl, _gold}
				});

			SetFontInfo(fontColor, backgroundColor);
			_gold.Font.Colour = Colour.Gold;
			var padding = _table.AddAutoRow();
			padding.Star();
		}

		private void SetFontInfo(Colour fontColor, Colour backgroundColor)
		{
			foreach(var row in _table.GetRows())
			{
				row.Auto();
			}

			foreach(var col in _table.GetColumns())
			{
				col.Star();
			}
			var index = 0;
			foreach (var cell in _table.GetCells())
			{
				var label = cell.Content.Control as Label;
				if (label != null)
				{
					label.Font.Colour = fontColor;
					label.Font.Large();
					label.Font.Bold();
					if(index % 2 == 1)
					{
						label.Alignment.CenterRight();
					}
					label.Background.Colour = backgroundColor;
				}
				var stack = cell.Content.Control as Stack;
				if (stack != null)
				{
					foreach (var panel in stack.GetPanels())
					{
						label = panel.Control as Label;
						if (label != null)
						{
							label.Font.Colour = fontColor;
							label.Font.Large();
							label.Font.Bold();
							if(index % 2 == 1)
							{
								label.Alignment.CenterRight();
							}
							label.Background.Colour = backgroundColor;
						}
					}
				}
				index++;
			}
		}

		public void SetData(Entity entity)
		{
			var alive = entity.GetComponent<Alive>();
			var name = entity.GetComponent<Name>();
			var attack = entity.GetComponent<AttackStat>();
			var defense = entity.GetComponent<DefenseStat>();
			var actor = entity.GetComponent<Actor>();

			if (alive != null)
			{
				_health.Text = alive.Health.ToString();
				_maxHealth.Text = alive.MaxHealth.ToString();
			}

			if(name != null)
			{
				_name.Text = name.NameString;
			}

			if(attack != null)
			{
				_attack.Text = attack.Power.ToString();
				_accuracy.Text = attack.Accuracy.ToString();
			}

			if(defense != null)
			{
				_defense.Text = defense.Chance.ToString();
			}

			if(actor != null)
			{
				_gold.Text = actor.Gold.ToString();
				_speed.Text = actor.Speed.ToString();
			}
		}
	}
}
