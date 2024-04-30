import React from "react"
import "./ProfilePicture.scss"
import { Flag } from "../icons/Flag"

type ProfilePictureProps = {
  id: string
  avatarUrl: string | null
  displayName: string
}

export function ProfilePicture(props: ProfilePictureProps) {
  return (
    <div key={props.id} className="profile-picture" title={props.displayName}>
      {props.avatarUrl ? (
        <img src={props.avatarUrl} alt={props.displayName} />
      ) : (
        <div className="profile-picture-default-icon">
          <Flag color="off-bg" />
        </div>
      )}
    </div>
  )
}
