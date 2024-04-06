import React from "react"

type BoardPreviewProps = {}

export function BoardPreview(props: BoardPreviewProps) {
  return <div></div>

  // <table cellPadding={0} cellSpacing={0}>
  //   <tbody>
  //     {Array.from({ length: 7 }).map((_, y) => (
  //       <tr key={`${userId}y${y}`}>
  //         {Array.from({ length: 7 }).map((_, x) => {
  //           const tileIndexHeight = settings.height / 7
  //           const tileIndexWidth = settings.width / 7

  //           const indices: number[] = []

  //           for (let k = y * tileIndexHeight; k < (y + 1) * tileIndexHeight; k++)
  //             for (let j = x * tileIndexWidth; j < (x + 1) * tileIndexWidth; j++)
  //               indices.push(i(Math.floor(j), Math.floor(k)))

  //           const isRevealed =
  //             indices.filter(i => State.isRevealed(state[i]) || State.isFlagged(state[i])).length >
  //             indices.length / 2

  //           return (
  //             <td key={userId + i(x, y)}>
  //               <div
  //                 className={`game-active-competitor-tile-${isRevealed ? "revealed" : "hidden"}`}
  //               />
  //             </td>
  //           )
  //         })}
  //       </tr>
  //     ))}
  //   </tbody>
  // </table>
}
