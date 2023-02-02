using System;
using System.Collections.ObjectModel;
using System.Reactive;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace NavBarPoC.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<SelectionHost> itemsCollection;

    public ReadOnlyObservableCollection<SelectionHost> ItemCollection => itemsCollection;

    public MainWindowViewModel()
    {
        SourceCache<Wallet, string> sourceCache = new(x => x.Name);

        sourceCache.AddOrUpdate(new Wallet { IsOpen = true, Name = "Wallet1",  });

        sourceCache
            .AsObservableCache()
            .Connect()
            .TransformWithInlineUpdate(
                newModel => new SelectionHost { Wallet = newModel }, 
                (existingVm, newModel) => existingVm.Wallet = newModel)
            .Sort(SortExpressionComparer<SelectionHost>.Ascending(wallet => wallet.SortId))
            .Bind(out itemsCollection)
            .Subscribe();

        AddWallet = ReactiveCommand.Create(() => sourceCache.AddOrUpdate(new Wallet
        {
            Name = NameText,
            SortId = Random.Shared.Next()
        }));
    }

    public ReactiveCommand<Unit, Unit> AddWallet { get; set; }

    public string NameText { get; set; } = "";
}

public class Wallet
{
    public string Name { get; set; }
    public bool IsOpen { get; set; }
    public int SortId { get; set; }
}

public class SelectionHost
{
    public Wallet Wallet { get; set; }
    public IComparable SortId => Wallet.SortId;
}