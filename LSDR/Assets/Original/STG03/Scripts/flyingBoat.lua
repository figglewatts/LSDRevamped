interacted = false

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, 0.7)
end

function interact()
    interacted = true
    DreamSystem.LogGraphContributionFromEntity(0, 4)
end