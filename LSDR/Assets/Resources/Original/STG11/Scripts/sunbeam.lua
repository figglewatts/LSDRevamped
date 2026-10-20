sunbeamTarget = GetEntity("SunbeamTarget").WorldPosition

moveSpeed = 2.5
moving = false
startPosition = this.GameObject.WorldPosition

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(2, 2)

    if moving then return end
    moving = true

    local first, second = sunbeamTarget, startPosition
    if Random.OneIn(2) then
        -- half of them go the other way first
        first, second = startPosition, sunbeamTarget
    end

    this.Action
        .Do(|| this.MoveTowards(first, moveSpeed))
        .Until(Condition.WaitForSeconds(15))
        .Then(|| this.MoveTowards(second, moveSpeed))
        .Until(Condition.WaitForSeconds(15))
        .ThenFinish()
end