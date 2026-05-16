import { httpClient } from "@/shared/api/httpClient";
import type {
  AddBoardRequest,
  AddBoardResponse,
  AddColumnRequest,
  BoardDto,
  GetBoardResponse,
  UpdateColumnRequest,
  MoveColumnRequest,
} from "../model/types";

export const boardsApi = {
  async getBoard(boardId: string): Promise<BoardDto> {
    const response = await httpClient.get<GetBoardResponse>(
      `/boards/${boardId}`,
    );

    return response.data.board;
  },

  async addBoard(projectId: string, data: AddBoardRequest): Promise<string> {
    const response = await httpClient.post<AddBoardResponse>(
      `/projects/${projectId}/boards`,
      data,
    );

    return response.data.boardId;
  },

  async addColumn(boardId: string, data: AddColumnRequest): Promise<string> {
    const response = await httpClient.post<string>(
      `/boards/${boardId}/columns`,
      data,
    );

    return response.data;
  },

  async updateColumn(
    boardId: string,
    columnId: string,
    data: UpdateColumnRequest,
  ): Promise<void> {
    await httpClient.patch(`/boards/${boardId}/columns/${columnId}`, data);
  },

  async moveColumn(
    boardId: string,
    columnId: string,
    data: MoveColumnRequest,
  ): Promise<void> {
    console.log("moveColumn request:", {
      url: `/boards/${boardId}/columns/${columnId}/move`,
      boardId,
      columnId,
      data,
    });
    await httpClient.patch(`/boards/${boardId}/columns/${columnId}/move`, data);
  },

  async deleteColumn(boardId: string, columnId: string): Promise<void> {
    await httpClient.delete(`/boards/${boardId}/columns/${columnId}`);
  },
};
