interacted = false
moveSpeed = 0.9

function start()
    local high = this.GameObject.Name ~= "HotAirBalloon1" and this.GameObject.Name ~= "HotAirBalloon2"
    if high and IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
    end
end

function update()
    if not interacted then return end

    this.MoveInDirection(this.Up, moveSpeed)
end

function interact()
    interacted = true
    this.LogGraphContribution(-2, 6)

    this.Action
        .WaitUntil(Condition.WaitForSeconds(90))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end