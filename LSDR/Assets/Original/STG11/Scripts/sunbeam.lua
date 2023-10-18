sunbeamTarget = GetEntity("SunbeamTarget").WorldPosition

moveSpeed = 3
startPosition = this.GameObject.WorldPosition

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(2, 2)

    if Random.OneIn(2) then
        this.Action
            .Do(|| this.MoveTowards(sunbeamTarget, moveSpeed))
            .Until(Condition.WaitForLinearMove(this.GameObject, sunbeamTarget))
            .Then(|| this.MoveTowards(startPosition, moveSpeed))
            .Until(Condition.WaitForLinearMove(this.GameObject, startPosition))
            .ThenLoop()
    end
end