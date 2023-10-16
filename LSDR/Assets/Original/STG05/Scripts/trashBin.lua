function interact()
    this.LogGraphContribution(0, -7)
    
    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .Then(|| this.StopAnimation())
        .ThenFinish()
end