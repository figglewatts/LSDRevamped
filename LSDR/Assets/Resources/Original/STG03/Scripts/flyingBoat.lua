interacted = false

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, 0.3)
end

function interact()
    interacted = true
    this.LogGraphContribution(0, 4)
end