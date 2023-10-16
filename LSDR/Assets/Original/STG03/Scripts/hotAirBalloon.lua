interacted = false
moveSpeed = 0.5

function update()
    if not interacted then return end

    this.MoveInDirection(this.Up, moveSpeed)
end

function interact()
    interacted = true
    this.LogGraphContribution(-2, 6)

    this.Action
        .WaitUntil(Condition.WaitForSeconds(60))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end