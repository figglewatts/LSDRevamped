interacted = false

function update()
    if not interacted then return end

    this.MoveInDirection(this.Up, 0.2)
end

function interact()
    this.LogGraphContribution(0, -1)

    if Random.OneIn(2) then return end
    this.PlayAnimation(0)
    interacted = true
end